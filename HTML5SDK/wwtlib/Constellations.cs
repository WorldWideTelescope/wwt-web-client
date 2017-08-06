using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Html.Media.Graphics;
using System.Runtime.CompilerServices;

namespace wwtlib
{
    public class Constellations
    {
        string name;
        static WebFile webFileConstNames;
        WebFile webFile;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        string url;
        public List<Lineset> lines;
        int pointCount = 0;
        bool boundry = false;
        bool noInterpollation = false;
        public bool ReadOnly = false;
        public static Constellations CreateBasic(string name)
        {
            Constellations temp = new Constellations();
            temp.name = name;
            temp.url = null;
            temp.lines = new List<Lineset>();
            foreach (string abbrv in FullNames.Keys)
            {
                temp.lines.Add(new Lineset(abbrv));
            }
            return temp;
        }


        public Constellations()
        {
        }

        public static Constellations Create(string name, string url, bool boundry, bool noInterpollation, bool resource)
        {
            Constellations temp = new Constellations();

            temp.noInterpollation = noInterpollation;
            temp.boundry = boundry;
            temp.name = name;
            temp.url = url;

            temp.GetFile();

            return temp;
        }

        public void GetFile()
        {
            webFile = new WebFile(url);
            webFile.OnStateChange = FileStateChange;
            webFile.Send();
        }

        public void FileStateChange()
        {
            if(webFile.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFile.Message);
            }
            else if(webFile.State == StateType.Received)
            {
                LoadConstellationData(webFile.GetText());
            }

        }

        private void LoadConstellationData(string data)
        {
            if (boundry && !noInterpollation)
            {
                boundries = new Dictionary<string, Lineset>();
            }
            lines = new List<Lineset>();



            Lineset lineSet = null; 


            try
            {
                string[] rows = data.Split("\r\n");


                    string abrv;
                    string abrvOld = "";
                    double ra;
                    double dec;
                    double lastRa = 0;
                    PointType type = PointType.Move;
                    foreach(string row in rows)
                    {
                        string line = row;

                        if (line.Substr(11, 2) == "- ")
                        {
                            line = line.Substr(0, 11) + " -" + line.Substr(13, (line.Length - 13));
                        }
                        if (line.Substr(11, 2) == "+ ")
                        {
                            line = line.Substr(0, 11) + " +" + line.Substr(13, (line.Length - 13));
                        }
                        dec = double.Parse(line.Substr(11, 10));
                        if (noInterpollation)
                        {
                            ra = double.Parse(line.Substr(0, 10));
                        }
                        else
                        {
                            ra = double.Parse(line.Substr(0, 10));
                        }

                        abrv = line.Substr(23, 4).Trim();
                        if (!boundry)
                        {
                            if (line.Substr(28, 1).Trim() != "")
                            {
                                type = (PointType)int.Parse(line.Substr(28, 1));
                            }
                        }
                        else
                        {
                            if (this.noInterpollation && line.Substr(28, 1) != "O")
                            {
                                continue;
                            }
                        }

                        //				if (abrv != abrvOld || type == PointType.Move)
                        if (abrv != abrvOld)
                        {
                            type = PointType.Start;
                            lineSet = new Lineset(abrv);
                            lines.Add(lineSet);
                            if (boundry && !noInterpollation)
                            {
                                boundries[abrv] =  lineSet;
                            }
                            abrvOld = abrv;
                            lastRa = 0;
                        }


                        if (this.noInterpollation)
                        {
                            if (Math.Abs(ra - lastRa) > 12)
                            {
                                ra = ra - (24 * ((ra - lastRa) < 0 ? -1 : 1));
                            }
                            lastRa = ra;
                            //console.WriteLine(String.Format("{0}, ra:{1}",abrv,ra));
                        }
                        string starName = null;
                        if (line.Length > 30)
                        {
                            starName = line.Substr(30).Trim();
                        }

                        if (starName == null || starName != "Empty")
                        {
                            lineSet.Add(ra, dec, type, starName);
                        }
                        pointCount++;
                        type = PointType.Line;

                    }
                    
                
            }
            catch
            {
                int i = 0;
            }
            WWTControl.RenderNeeded = true;
        }


        protected const double RC = 0.017453292519943;
        protected double radius = 1.0f;


        public void Draw(RenderContext renderContext, bool showOnlySelected, string focusConsteallation, bool clearExisting)
        {
            maxSeperation = Math.Max(.6, Math.Cos((renderContext.FovAngle * 2) / 180.0 * Math.PI));

            drawCount = 0;
            Lineset lsSelected = null;
            if (lines == null || ConstellationCentroids == null)
            {
                return;
            }

            constToDraw = focusConsteallation;


            foreach (Lineset ls in this.lines)
            {
                if (constToDraw == ls.Name && boundry)
                {
                    lsSelected = ls;
                }
                else if (!showOnlySelected || !boundry)
                {
                    DrawSingleConstellation(renderContext, ls, 1f);
                }
            }

            if (lsSelected != null)
            {
                DrawSingleConstellation(renderContext, lsSelected, 1f);

            }
        }
        int drawCount = 0;
        static double maxSeperation = .745;

        Dictionary<string, SimpleLineList> constellationVertexBuffers = new Dictionary<string, SimpleLineList>();

        private void DrawSingleConstellation(RenderContext renderContext, Lineset ls, float opacity)
        {
            bool reverse = false;
            Place centroid = ConstellationCentroids[ls.Name];
            if (centroid != null)
            {
                Vector3d pos = Coordinates.RADecTo3d(reverse ? -centroid.RA - 6 : centroid.RA, reverse ? centroid.Dec : centroid.Dec);

                if (Vector3d.Dot(renderContext.ViewPoint, pos) < maxSeperation)
                {
                    return;
                }
            }

            if (!constellationVertexBuffers.ContainsKey(ls.Name))
            {
                int count = ls.Points.Count;

                SimpleLineList linelist = new SimpleLineList();
                linelist.DepthBuffered = false;
                constellationVertexBuffers[ls.Name] = linelist;

                Vector3d currentPoint = new Vector3d();
                Vector3d temp;

                for (int i = 0; i < count; i++)
                {

                    if (ls.Points[i].PointType == PointType.Move || i == 0)
                    {
                        currentPoint = Coordinates.RADecTo3d(ls.Points[i].RA, ls.Points[i].Dec);
                    }
                    else
                    {
                        temp = Coordinates.RADecTo3d(ls.Points[i].RA, ls.Points[i].Dec);
                        linelist.AddLine(currentPoint, temp);
                        currentPoint = temp;
                    }
                }

                if (boundry)
                {
                    temp = Coordinates.RADecTo3d(ls.Points[0].RA, ls.Points[0].Dec);
                    linelist.AddLine(currentPoint, temp);
                }
            }

            string col = "red";
            if (boundry)
            {
                if (constToDraw != ls.Name)
                {
                    col = Settings.GlobalSettings.ConstellationBoundryColor;
                }
                else
                {
                    col = Settings.GlobalSettings.ConstellationSelectionColor;
                }
            }
            else
            {
                col = Settings.GlobalSettings.ConstellationFigureColor;
            }

            constellationVertexBuffers[ls.Name].DrawLines(renderContext, opacity, Color.Load(col));

        }

        //protected Vector3d RaDecTo3d(double lat, double lng)
        //{
        //    return Vector3d.Create((Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));

        //}
        private void DrawSingleConstellationOld(RenderContext renderContext, Lineset ls)
        {
            bool reverse = false;
            // todo get this working
            Place centroid = ConstellationCentroids[ls.Name];
            if (centroid != null)
            {
                Vector3d pos = Coordinates.RADecTo3d(reverse ? -centroid.RA - 6 : centroid.RA, reverse ? centroid.Dec : centroid.Dec);

                if (Vector3d.Dot(renderContext.ViewPoint, pos) < maxSeperation)
                {
                    return;
                }
            }

            drawCount++;
            string col;
            if (boundry)
            {
                if (constToDraw != ls.Name)
                {
                    col = Settings.GlobalSettings.ConstellationBoundryColor;
                }
                else
                {
                    col = Settings.GlobalSettings.ConstellationSelectionColor;
                }
            }
            else
            {
                col = Settings.GlobalSettings.ConstellationFigureColor;
            }

            if (renderContext.gl == null)
            {
                CanvasContext2D ctx = renderContext.Device;

                int count = ls.Points.Count;

                Vector3d lastPoint = new Vector3d();
                ctx.Save();
                bool linePending = false;
                ctx.BeginPath();
                ctx.StrokeStyle = col;
                ctx.LineWidth = 2;
                ctx.Alpha = .25;
                for (int i = 0; i < count; i++)
                {


                    if (ls.Points[i].PointType == PointType.Move || i == 0)
                    {
                        if (linePending)
                        {
                            ctx.Stroke();
                        }
                        lastPoint = renderContext.WVP.Transform(Coordinates.RADecTo3d(ls.Points[i].RA, ls.Points[i].Dec));

                        ctx.MoveTo(lastPoint.X, lastPoint.Y);
                    }
                    else
                    {
                        Vector3d newPoint = renderContext.WVP.Transform(Coordinates.RADecTo3d(ls.Points[i].RA, ls.Points[i].Dec));

                        //            if (lastPoint.Z > 0 && newPoint.Z > 0)
                        {
                            ctx.LineTo(newPoint.X, newPoint.Y);
                            linePending = true;
                        }
                    }
                }

                if (boundry)
                {
                    ctx.ClosePath();
                }

                ctx.Stroke();
                ctx.Restore();
            }
            else
            {
                //todo add webgl method of drawing
            }
        }


        public static Constellations Containment = Constellations.Create("Constellations", "http://www.worldwidetelescope.org/data/constellations.txt", true, true, true);
        //public static Constellations Containment = Constellations.Create("Constellations", "http://localhost/data/constellations.txt", true, true, true);

        static string constToDraw = "";

        public static Linepoint SelectedSegment = null;

        public string FindConstellationForPoint(double ra, double dec)
        {
            if (dec > 88.402 || this.lines == null)
            {
                return "UMI";
            }

            foreach (Lineset ls in this.lines)
            {
                int count = ls.Points.Count;

                int i;
                int j;
                bool inside = false;
                for (i = 0, j = count - 1; i < count; j = i++)
                {

                    if ((((ls.Points[i].Dec <= dec) && (dec < ls.Points[j].Dec)) ||
                     ((ls.Points[j].Dec <= dec) && (dec < ls.Points[i].Dec))) &&
                    (ra < (ls.Points[j].RA - ls.Points[i].RA) * (dec - ls.Points[i].Dec) / (ls.Points[j].Dec - ls.Points[i].Dec) + ls.Points[i].RA))
                    {

                        inside = !inside;
                    }
                }
                if (inside)
                {
                    //constToDraw = ls.Name;
                    return ls.Name;
                }
            }
            if (ra > 0)
            {
                return FindConstellationForPoint(ra - 24, dec);
            }

            // Ursa Minor is tricky since it wraps around the poles. It can evade the point in rect test
            if (dec > 65.5)
            {
                return "UMI";
            }
            if (dec < -65.5)
            {
                return "OCT";
            }
            return "Error";
        }

        static Text3dBatch NamesBatch;
        public static void DrawConstellationNames(RenderContext renderContext, float opacity, Color drawColor)
        {
            if (NamesBatch == null)
            {
            
                InitializeConstellationNames();
                if (NamesBatch == null)
                {
                    return;
                }
            }
            NamesBatch.Draw(renderContext, opacity, drawColor);
        }

        public static void InitializeConstellationNames()
        {
            if (ConstellationCentroids == null)
            {
                return;
            }


            NamesBatch = new Text3dBatch(80);


            foreach (string key in ConstellationCentroids.Keys)
            {
                IPlace centroid = ConstellationCentroids[key];
            
                Vector3d center = Coordinates.RADecTo3dAu(centroid.RA, centroid.Dec, 1);
                Vector3d up = Vector3d.Create(0, 1, 0);
                string name = centroid.Name;

                if (centroid.Name == "Triangulum Australe")
                {
                    name = name.Replace(" ", "\n   ");
                }
                NamesBatch.Add(new Text3d(center, up, name, 80, .000125));
            }
        }



        static Folder artFile = null;
        public static List<Place> Artwork = null;
        public static void DrawArtwork(RenderContext renderContext)
        {
            if (Artwork == null)
            {
                if (artFile == null)
                {
                    artFile = new Folder();
                    artFile.LoadFromUrl("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?W=hevelius", OnArtReady);
                    //artFile.LoadFromUrl("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?W=and", OnArtReady);
                }

                return;
            }
            maxSeperation = Math.Max(.50, Math.Cos((renderContext.FovAngle * 2) / 180.0 * Math.PI));


            foreach (Place place in Artwork)
            {
                BlendState bs = PictureBlendStates[place.Constellation];
                bs.TargetState = Settings.Active.ConstellationArtFilter.IsSet(place.Constellation);

                if (bs.State)
                {
                    bool reverse = false;
                    Place centroid = ConstellationCentroids[place.Constellation];
                    if (centroid != null)
                    {
                        Vector3d pos = Coordinates.RADecTo3d(reverse ? -centroid.RA - 6 : centroid.RA, reverse ? centroid.Dec : centroid.Dec);

                        if (Vector3d.Dot(renderContext.ViewPoint, pos) > maxSeperation)
                        {

                            renderContext.DrawImageSet(place.StudyImageset, 100);
                        }

                    }
                }
            }
        }
        static void OnArtReady()
        {
            artFile.ChildLoadCallback(LoadArtList);

        }
        static void LoadArtList()
        {
            Artwork = artFile.Places;

        }

        public static Dictionary<string, Lineset> boundries = null;
        public static Dictionary<String, String> FullNames;
        public static Dictionary<String, String> Abbreviations;
        public static Dictionary<string, Place> ConstellationCentroids;
        public static Dictionary<string, BlendState> PictureBlendStates = new Dictionary<string, BlendState>();


        static Constellations()
        {
            //string url = "http://www.worldwidetelescope.org/data/constellationNames_RADEC_EN.txt";
            //string url = "http://localhost/data/constellationNames_RADEC_EN.txt";
            string url = "http://www.worldwidetelescope.org/wwtweb/catalog.aspx?q=ConstellationNamePositions_EN";
            webFileConstNames = new WebFile(url);
            webFileConstNames.OnStateChange = LoadNames;
            webFileConstNames.Send();

        }

        static void LoadNames()
        {
            if (webFileConstNames.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFileConstNames.Message);
            }
            else if (webFileConstNames.State == StateType.Received)
            {
                CentroidsReady(webFileConstNames.GetText());
            }
        }
        public static Dictionary<String, int> BitIDs;
        static void CentroidsReady(string file)
        {
            ConstellationCentroids = new Dictionary<string, Place>();
            FullNames = new Dictionary<string, string>();
            Abbreviations = new Dictionary<string, string>();
            BitIDs = new Dictionary<string, int>();
            string[] rows = file.Split("\r\n");
            int id = 0;        
            string line;
            foreach (string row in rows)
            {
                line = row;
                string[] data = line.Split( ",");
                FullNames[data[1]] = data[0];
                Abbreviations[data[0]] = data[1];
                BitIDs[data[1]] =  id++;
                PictureBlendStates[data[1]] = BlendState.Create(true, 1000);
                ConstellationCentroids[data[1]] = Place.Create(data[0], double.Parse(data[3]), double.Parse(data[2]), Classification.Constellation, data[1], ImageSetType.Sky, 360);
            }
            WWTControl.RenderNeeded = true;
            ConstellationFilter.BuildConstellationFilters();
        }

        static public string FullName(string name)
        {
            if (FullNames.ContainsKey(name))
            {
                return FullNames[name];
            }
            return name;
        }
        static public string Abbreviation(string name)
        {
            if (Abbreviations != null && !String.IsNullOrEmpty(name) && Abbreviations.ContainsKey(name))
            {
                return Abbreviations[name];
            }

            return name;
        }


    }

    public class Lineset
    {
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Linepoint> Points;

        public Lineset(string name)
        {
            this.name = name;

            Points = new List<Linepoint>();
        }

        public void Add(double ra, double dec, PointType pointType, string name)
        {
            Points.Add(new Linepoint(ra, dec, pointType, name));
        }
    }
    public class Linepoint
    {
        public double RA;
        public double Dec;
        public PointType PointType;
        public string Name = null;

        public Linepoint(double ra, double dec, PointType type, string name)
        {
            RA = ra;
            Dec = dec;
            PointType = type;
            Name = name;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return Coordinates.FormatDMS((((((RA)) / 360) * 24.0 + 12) % 24)) + ", " + Coordinates.FormatDMS(Dec) + ", " + PointType.ToString();
            }
            else
            {
                return Name + ", " + PointType.ToString();
            }
        }
    }
    public enum PointType { Move=0, Line=1, Dash=2, Start=3 };


    public class ConstellationFilter
    {
        public Int32[] Bits = new Int32[3];
        public Int32[] OldBits = new Int32[3];
        public BlendState BlendState = BlendState.Create(false, 1000);
        public bool Internal = false;
        public ConstellationFilter()
        {
            for (int i = 0; i < 3; i++)
            {
                Bits[i] = ~ Bits[i];
                OldBits[i] = Bits[i];
            }
        }
        private void SaveBits()
        {
            for (int i = 0; i < 3; i++)
            {
                OldBits[i] = Bits[i];
            }
        }

        private bool IsChanged()
        {
            for (int i = 0; i < 3; i++)
            {
                if (OldBits[i] != Bits[i])
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckChanged()
        {
            if (IsChanged())
            {
                FireChanged();
            }
        }

        public bool IsEnabled(string abbrev)
        {
            Int32 bitID = Constellations.BitIDs[abbrev];

            int index = bitID / 32;
            bitID = bitID % 32;

            return BlendState.State && ((1 << (bitID)) & Bits[index]) != 0;
        }

        public bool IsSet(string abbrev)
        {
            SaveBits();

            Int32 bitID = Constellations.BitIDs[abbrev];

            int index = (int) (bitID / 32);
            bitID = bitID % 32;

            return ((1 << (bitID)) & Bits[index]) != 0;
        }

        public void Set(string abbrev, bool state)
        {
            SaveBits();
            int bitID = Constellations.BitIDs[abbrev];

            int index = bitID / 32;
            bitID = bitID % 32;

            if (state)
            {
                Bits[index] = Bits[index] | (1 << bitID);
            }
            else
            {
                Bits[index] = Bits[index] ^ (1 << bitID);
            }

            CheckChanged();
        }

        public void SetAll(bool state)
        {
            SaveBits();
            for (int bitID = 0; bitID < 89; bitID++)
            {

                int index = bitID / 32;
                int bit = bitID % 32;

                if (state)
                {
                    Bits[index] = Bits[index] | (1 << bit);
                }
                else
                {
                    Bits[index] = Bits[index] ^ (1 << bit);
                }
            }
            CheckChanged();
        }

        public void SetBits(byte[] bits)
        {
            SaveBits();
            for (int i = 0; i < 3; i++)
            {
                Bits[i] = ((int)bits[i * 4]) + (((int)bits[i * 4 + 1]) << 8) + (((int)bits[i * 4 + 2]) << 16) + (((int)bits[i * 4 + 3]) << 24);
            }
            CheckChanged();
        }

        public byte[] GetBits()
        {
            byte[] bits = new byte[12];

            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                bits[index++] = (byte)(Bits[i]);
                bits[index++] = (byte)(Bits[i] >> 8);
                bits[index++] = (byte)(Bits[i] >> 16);
                bits[index++] = (byte)(Bits[i] >> 24);
            }

            return bits;

        }

        public void CloneFilter(ConstellationFilter filter)
        {
            SaveBits();
            for (int i = 0; i < 3; i++)
            {
                Bits[i] = filter.Bits[i];
            }
            CheckChanged();
        }

        public ConstellationFilter Clone()
        {
            ConstellationFilter newFilter = new ConstellationFilter();
            newFilter.CloneFilter(this);
            return newFilter;
        }

        public void Combine(ConstellationFilter filter)
        {
            SaveBits();
            for (int i = 0; i < 3; i++)
            {
                Bits[i] = Bits[i] | filter.Bits[i];
            }
            CheckChanged();
        }

        public void Remove(ConstellationFilter filter)
        {
            SaveBits();
            for (int i = 0; i < 3; i++)
            {
                Bits[i] = Bits[i] & ~filter.Bits[i];
            }
            CheckChanged();
        }

        public static Dictionary<string, ConstellationFilter> Families = new Dictionary<string, ConstellationFilter>();

        public static void BuildConstellationFilters()
        {
            ConstellationFilter all = AllConstellation;
            all.Internal = true;
            Families["AllConstellation"] =  all;
            Families["Zodiacal"] = Zodiacal;
            Families["Ursa Major Family"]= UrsaMajorFamily;
            Families["Perseus Family"]=  PerseusFamily;
            Families["Hercules Family"]=  HerculesFamily;
            Families["Orion Family"]=  OrionFamily;
            Families["Heavenly Waters"]=  HeavenlyWaters;
            Families["Bayer Family"]=  BayerFamily;
            Families["La Caille Family"]=  LaCaileFamily;
            //LoadCustomFilters();
        }

        public static void SaveCustomFilters()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, ConstellationFilter> kv in Families)
            {
                if (!kv.Value.Internal)
                {
                    sb.Append(kv.Key);
                    sb.Append(";");
                    sb.AppendLine(kv.Value.ToString());

                }
            }

            //Properties.Settings.Default.SavedFilters = sb.ToString();
        }

        //public static void LoadCustomFilters()
        //{
        //    string[] lines = Properties.Settings.Default.SavedFilters.Split(new char[] { '\n' });
        //    foreach (string line in lines)
        //    {
        //        try
        //        {
        //            string[] parts = line.Split(new char[] { ';' });
        //            if (parts.Length > 1)
        //            {
        //                ConstellationFilter filter = ConstellationFilter.Parse(parts[1]);
        //                Families.Add(parts[0], filter);
        //            }
        //        }
        //        catch
        //        {
        //        }
        //    }
        //}

        public static ConstellationFilter AllConstellation
        {
            get
            {
                ConstellationFilter all = new ConstellationFilter();

                all.SetAll(true);

                return all;
            }

        }

        public static ConstellationFilter Zodiacal
        {
            get
            {
                ConstellationFilter zodiacal = new ConstellationFilter();
                zodiacal.Set("ARI", true);
                zodiacal.Set("TAU", true);
                zodiacal.Set("GEM", true);
                zodiacal.Set("CNC", true);
                zodiacal.Set("LEO", true);
                zodiacal.Set("VIR", true);
                zodiacal.Set("LIB", true);
                zodiacal.Set("SCO", true);
                zodiacal.Set("SGR", true);
                zodiacal.Set("CAP", true);
                zodiacal.Set("AQR", true);
                zodiacal.Set("PSC", true);
                zodiacal.Internal = true;
                return zodiacal;
            }

        }

        public static ConstellationFilter UrsaMajorFamily
        {
            get
            {
                ConstellationFilter uma = new ConstellationFilter();
                uma.Set("UMA", true);
                uma.Set("UMI", true);
                uma.Set("DRA", true);
                uma.Set("CVN", true);
                uma.Set("BOO", true);
                uma.Set("COM", true);
                uma.Set("CRB", true);
                uma.Set("CAM", true);
                uma.Set("LYN", true);
                uma.Set("LMI", true);
                uma.Internal = true;
                return uma;
            }
        }

        public static ConstellationFilter PerseusFamily
        {
            get
            {
                ConstellationFilter Perseus = new ConstellationFilter();
                Perseus.Set("CAS", true);
                Perseus.Set("CEP", true);
                Perseus.Set("AND", true);
                Perseus.Set("PER", true);
                Perseus.Set("PEG", true);
                Perseus.Set("CET", true);
                Perseus.Set("AUR", true);
                Perseus.Set("LAC", true);
                Perseus.Set("TRI", true);
                Perseus.Internal = true;

                return Perseus;
            }

        }

        public static ConstellationFilter HerculesFamily
        {
            get
            {
                ConstellationFilter hercules = new ConstellationFilter();
                hercules.Set("HER", true);
                hercules.Set("SGE", true);
                hercules.Set("AQL", true);
                hercules.Set("LYR", true);
                hercules.Set("CYG", true);
                hercules.Set("VUL", true);
                hercules.Set("HYA", true);
                hercules.Set("SEX", true);
                hercules.Set("CRT", true);
                hercules.Set("CRV", true);
                hercules.Set("OPH", true);
                hercules.Set("SER1", true);
                hercules.Set("SER2", true);
                hercules.Set("SCT", true);
                hercules.Set("CEN", true);
                hercules.Set("LUP", true);
                hercules.Set("CRA", true);
                hercules.Set("ARA", true);
                hercules.Set("TRA", true);
                hercules.Set("CRU", true);
                hercules.Internal = true;

                return hercules;
            }

        }
        public static ConstellationFilter OrionFamily
        {
            get
            {
                ConstellationFilter orion = new ConstellationFilter();
                orion.Set("ORI", true);
                orion.Set("CMA", true);
                orion.Set("CMI", true);
                orion.Set("MON", true);
                orion.Set("LEP", true);
                orion.Internal = true;

                return orion;
            }

        }
        public static ConstellationFilter HeavenlyWaters
        {
            get
            {
                ConstellationFilter waters = new ConstellationFilter();
                waters.Set("DEL", true);
                waters.Set("EQU", true);
                waters.Set("ERI", true);
                waters.Set("PSA", true);
                waters.Set("CAR", true);
                waters.Set("PUP", true);
                waters.Set("VEL", true);
                waters.Set("PYX", true);
                waters.Set("COL", true);
                waters.Internal = true;
                return waters;
            }
        }
        public static ConstellationFilter BayerFamily
        {
            get
            {
                ConstellationFilter bayer = new ConstellationFilter();
                bayer.Set("HYA", true);
                bayer.Set("DOR", true);
                bayer.Set("VOL", true);
                bayer.Set("APS", true);
                bayer.Set("PAV", true);
                bayer.Set("GRU", true);
                bayer.Set("PHE", true);
                bayer.Set("TUC", true);
                bayer.Set("IND", true);
                bayer.Set("CHA", true);
                bayer.Set("MUS", true);
                bayer.Internal = true;
                return bayer;
            }

        }
        public static ConstellationFilter LaCaileFamily
        {
            get
            {
                ConstellationFilter LaCaile = new ConstellationFilter();
                LaCaile.Set("NOR", true);
                LaCaile.Set("CIR", true);
                LaCaile.Set("TEL", true);
                LaCaile.Set("MIC", true);
                LaCaile.Set("SCL", true);
                LaCaile.Set("FOR", true);
                LaCaile.Set("CAE", true);
                LaCaile.Set("HOR", true);
                LaCaile.Set("OCT", true);
                LaCaile.Set("MEN", true);
                LaCaile.Set("RET", true);
                LaCaile.Set("PIC", true);
                LaCaile.Set("ANT", true);
                LaCaile.Internal = true;
                return LaCaile;
            }

        }

        public bool SettingsOwned = false;
        private void FireChanged()
        {
            if (SettingsOwned)
            {
             //   Properties.Settings.Default.PulseMeForUpdate = !Properties.Settings.Default.PulseMeForUpdate;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", Bits[0], Bits[1], Bits[2]);
        }

        public static ConstellationFilter Parse(string val)
        {
            string[] parts = ((string)val).Split(",");

            ConstellationFilter cf = new ConstellationFilter();
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    cf.Bits[i] = Int32.Parse(parts[i]);
                }
            }
            catch
            {
            }

            return cf;
        }
    }
}
