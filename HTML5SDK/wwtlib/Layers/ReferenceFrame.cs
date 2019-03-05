using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;


namespace wwtlib
{
    public enum ReferenceFrameTypes { FixedSherical=0, Orbital=1, Trajectory = 2, Synodic = 3 /*,FixedRectangular*/ };
    public class ReferenceFrame
    {
        public ReferenceFrame()
        {
            WorldMatrix = new Matrix3d();
            WorldMatrix = Matrix3d.Identity;
        }

        internal bool SystemGenerated = false;
        // Calclulated
        public Vector3d Position;
        public double MeanAnomoly;
        public Matrix3d WorldMatrix;
        public double OrbitalYears;

        public bool ObservingLocation = false;

        // Serialized
        public string Name;
        public string Parent;
        public ReferenceFrames Reference = ReferenceFrames.Custom;
        public bool ParentsRoationalBase = false;
        public ReferenceFrameTypes ReferenceFrameType = ReferenceFrameTypes.FixedSherical;
        public double MeanRadius = 6371000;
        public double Oblateness = 0.0033528;
        public double Heading;
        public double Pitch;
        public double Roll;
        public double Scale = 1;
        public double Tilt;
        public Vector3d Translation = new Vector3d();

        // For Sphetical Offset
        public double Lat;
        public double Lng;
        public double Altitude;
        // For Rotating frames
        public double RotationalPeriod; // Days
        public double ZeroRotationDate; // Julian decimal

        // For representing orbits & distant point location
        public Color representativeColor = Colors.White; // Used for orbits and points
        public Color RepresentativeColor
        {
            get
            {
                return representativeColor;
            }
            set
            {
                if (value != representativeColor)
                {
                    representativeColor = value;
                    //todo add lines back
                    //trajectoryLines = null;
                    orbit = null;
                }
            }
        }
        public bool ShowAsPoint = false;
        public bool ShowOrbitPath = false;
        public bool StationKeeping = true;

        public double SemiMajorAxis; // a Au's
        public AltUnits SemiMajorAxisUnits = AltUnits.Meters;
        public double Eccentricity; // e
        public double Inclination; // i
        public double ArgumentOfPeriapsis; // w
        public double LongitudeOfAscendingNode; // Omega
        public double MeanAnomolyAtEpoch; // M
        public double MeanDailyMotion; // n .degrees day
        public double Epoch; //   Standard Equinox
        private Orbit orbit = null;

        public Orbit Orbit
        {
            get
            {
                return orbit;
            }
            set { orbit = value; }
        }
        //todo add this back
        //public LineList trajectoryLines = null;
        EOE elements = new EOE();

        public string GetIndentifier()
        {
            return Name;
        }

        //  public List<TrajectorySample> Trajectory = new List<TrajectorySample>();

        public void ImportTrajectory(string filename)
        {
            //Trajectory.Clear();
            //string[] data = File.ReadAllLines(filename);
            //foreach (string line in data)
            //{
            //    Trajectory.Add(new TrajectorySample(line));
            //}
        }

        //public virtual void SaveToXml(System.Xml.XmlTextWriter xmlWriter)
        //{
        //    xmlWriter.WriteStartElement("ReferenceFrame");
        //    xmlWriter.WriteAttributeString("Name", Name);
        //    xmlWriter.WriteAttributeString("Parent", Parent);
        //    xmlWriter.WriteAttributeString("ReferenceFrameType", ReferenceFrameType.ToString());
        //    xmlWriter.WriteAttributeString("Reference", Reference.ToString());
        //    xmlWriter.WriteAttributeString("ParentsRoationalBase",ParentsRoationalBase.ToString() );
        //    xmlWriter.WriteAttributeString("MeanRadius", MeanRadius.ToString());
        //    xmlWriter.WriteAttributeString("Oblateness", Oblateness.ToString());
        //    xmlWriter.WriteAttributeString("Heading", Heading.ToString());
        //    xmlWriter.WriteAttributeString("Pitch", Pitch.ToString());
        //    xmlWriter.WriteAttributeString("Roll", Roll.ToString());
        //    xmlWriter.WriteAttributeString("Scale", Scale.ToString());
        //    xmlWriter.WriteAttributeString("Tilt", Tilt.ToString());
        //    xmlWriter.WriteAttributeString("Translation", Translation.ToString());
        //    if (ReferenceFrameType == ReferenceFrameTypes.FixedSherical)
        //    {
        //        xmlWriter.WriteAttributeString("Lat", Lat.ToString());
        //        xmlWriter.WriteAttributeString("Lng", Lng.ToString());
        //        xmlWriter.WriteAttributeString("Altitude", Altitude.ToString());
        //    }
        //    xmlWriter.WriteAttributeString("RotationalPeriod", RotationalPeriod.ToString());
        //    xmlWriter.WriteAttributeString("ZeroRotationDate", ZeroRotationDate.ToString());
        //    xmlWriter.WriteAttributeString("RepresentativeColor",SavedColor.Save(RepresentativeColor));
        //    xmlWriter.WriteAttributeString("ShowAsPoint", ShowAsPoint.ToString());
        //    xmlWriter.WriteAttributeString("StationKeeping", StationKeeping.ToString());
        //    if (ReferenceFrameType == ReferenceFrameTypes.Orbital)
        //    {
        //        xmlWriter.WriteAttributeString("ShowOrbitPath", ShowOrbitPath.ToString());
        //        xmlWriter.WriteAttributeString("SemiMajorAxis", SemiMajorAxis.ToString());
        //        xmlWriter.WriteAttributeString("SemiMajorAxisScale", this.SemiMajorAxisUnits.ToString());
        //        xmlWriter.WriteAttributeString("Eccentricity", Eccentricity.ToString());
        //        xmlWriter.WriteAttributeString("Inclination", Inclination.ToString());
        //        xmlWriter.WriteAttributeString("ArgumentOfPeriapsis", ArgumentOfPeriapsis.ToString());
        //        xmlWriter.WriteAttributeString("LongitudeOfAscendingNode", LongitudeOfAscendingNode.ToString());
        //        xmlWriter.WriteAttributeString("MeanAnomolyAtEpoch", MeanAnomolyAtEpoch.ToString());
        //        xmlWriter.WriteAttributeString("MeanDailyMotion", MeanDailyMotion.ToString());
        //        xmlWriter.WriteAttributeString("Epoch", Epoch.ToString());
        //    }

        //    xmlWriter.WriteEndElement();
        //}

        public virtual void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ReferenceFrame");
            xmlWriter.WriteAttributeString("Name", Name);
            xmlWriter.WriteAttributeString("Parent", Parent);
            xmlWriter.WriteAttributeString("ReferenceFrameType", Enums.ToXml("ReferenceFrameTypes", (int)ReferenceFrameType));
            xmlWriter.WriteAttributeString("Reference", Enums.ToXml("ReferenceFrames",(int)Reference));
            xmlWriter.WriteAttributeString("ParentsRoationalBase", ParentsRoationalBase.ToString());
            xmlWriter.WriteAttributeString("MeanRadius", MeanRadius.ToString());
            xmlWriter.WriteAttributeString("Oblateness", Oblateness.ToString());
            xmlWriter.WriteAttributeString("Heading", Heading.ToString());
            xmlWriter.WriteAttributeString("Pitch", Pitch.ToString());
            xmlWriter.WriteAttributeString("Roll", Roll.ToString());
            xmlWriter.WriteAttributeString("Scale", Scale.ToString());
            xmlWriter.WriteAttributeString("Tilt", Tilt.ToString());
            xmlWriter.WriteAttributeString("Translation", Translation.ToString());
            if (ReferenceFrameType == ReferenceFrameTypes.FixedSherical)
            {
                xmlWriter.WriteAttributeString("Lat", Lat.ToString());
                xmlWriter.WriteAttributeString("Lng", Lng.ToString());
                xmlWriter.WriteAttributeString("Altitude", Altitude.ToString());
            }
            xmlWriter.WriteAttributeString("RotationalPeriod", RotationalPeriod.ToString());
            xmlWriter.WriteAttributeString("ZeroRotationDate", ZeroRotationDate.ToString());
            xmlWriter.WriteAttributeString("RepresentativeColor", RepresentativeColor.Save());
            xmlWriter.WriteAttributeString("ShowAsPoint", ShowAsPoint.ToString());
            xmlWriter.WriteAttributeString("ShowOrbitPath", ShowOrbitPath.ToString());

            xmlWriter.WriteAttributeString("StationKeeping", StationKeeping.ToString());

            if (ReferenceFrameType == ReferenceFrameTypes.Orbital)
            {
                xmlWriter.WriteAttributeString("SemiMajorAxis", SemiMajorAxis.ToString());
                xmlWriter.WriteAttributeString("SemiMajorAxisScale", Enums.ToXml("AltUnits", (int)SemiMajorAxisUnits));
                xmlWriter.WriteAttributeString("Eccentricity", Eccentricity.ToString());
                xmlWriter.WriteAttributeString("Inclination", Inclination.ToString());
                xmlWriter.WriteAttributeString("ArgumentOfPeriapsis", ArgumentOfPeriapsis.ToString());
                xmlWriter.WriteAttributeString("LongitudeOfAscendingNode", LongitudeOfAscendingNode.ToString());
                xmlWriter.WriteAttributeString("MeanAnomolyAtEpoch", MeanAnomolyAtEpoch.ToString());
                xmlWriter.WriteAttributeString("MeanDailyMotion", MeanDailyMotion.ToString());
                xmlWriter.WriteAttributeString("Epoch", Epoch.ToString());
            }


            //todo add this back when we support trajectories
            //if (ReferenceFrameType == ReferenceFrameTypes.Trajectory)
            //{
            //    xmlWriter.WriteStartElement("Trajectory");

            //    foreach (TrajectorySample sample in Trajectory)
            //    {
            //        string data = sample.ToString();
            //        xmlWriter.WriteElementString("Sample", data);
            //    }
            //    xmlWriter.WriteEndElement();
            //}

            xmlWriter.WriteEndElement();
        }


        public virtual void InitializeFromXml(XmlNode node)
        {
            Name = node.Attributes.GetNamedItem("Name").Value;
            Parent = node.Attributes.GetNamedItem("Parent").Value;
            ReferenceFrameType = (ReferenceFrameTypes)Enums.Parse("ReferenceFrameTypes", node.Attributes.GetNamedItem("ReferenceFrameType").Value);

            Reference = (ReferenceFrames)Enums.Parse("ReferenceFrames", node.Attributes.GetNamedItem("Reference").Value);

            ParentsRoationalBase = Boolean.Parse(node.Attributes.GetNamedItem("ParentsRoationalBase").Value);
            MeanRadius = Double.Parse(node.Attributes.GetNamedItem("MeanRadius").Value);
            Oblateness = Double.Parse(node.Attributes.GetNamedItem("Oblateness").Value);
            Heading = Double.Parse(node.Attributes.GetNamedItem("Heading").Value);
            Pitch = Double.Parse(node.Attributes.GetNamedItem("Pitch").Value);
            Roll = Double.Parse(node.Attributes.GetNamedItem("Roll").Value);
            Scale = Double.Parse(node.Attributes.GetNamedItem("Scale").Value);
            Tilt = Double.Parse(node.Attributes.GetNamedItem("Tilt").Value);
            Translation = Vector3d.Parse(node.Attributes.GetNamedItem("Translation").Value);
            if (ReferenceFrameType == ReferenceFrameTypes.FixedSherical)
            {
                Lat = Double.Parse(node.Attributes.GetNamedItem("Lat").Value);
                Lng = Double.Parse(node.Attributes.GetNamedItem("Lng").Value);
                Altitude = Double.Parse(node.Attributes.GetNamedItem("Altitude").Value);
            }

            RotationalPeriod = Double.Parse(node.Attributes.GetNamedItem("RotationalPeriod").Value);
            ZeroRotationDate = Double.Parse(node.Attributes.GetNamedItem("ZeroRotationDate").Value);
            RepresentativeColor = Color.Load(node.Attributes.GetNamedItem("RepresentativeColor").Value);
            ShowAsPoint = Boolean.Parse(node.Attributes.GetNamedItem("ShowAsPoint").Value);
            if (node.Attributes.GetNamedItem("StationKeeping")!= null)
            {
                StationKeeping = Boolean.Parse(node.Attributes.GetNamedItem("StationKeeping").Value);
            }

            if (ReferenceFrameType == ReferenceFrameTypes.Orbital)
            {
                ShowOrbitPath = Boolean.Parse(node.Attributes.GetNamedItem("ShowOrbitPath").Value);
                SemiMajorAxis = Double.Parse(node.Attributes.GetNamedItem("SemiMajorAxis").Value);

                SemiMajorAxisUnits = (AltUnits)Enums.Parse("AltUnits", node.Attributes.GetNamedItem("SemiMajorAxisScale").Value);

                Eccentricity = Double.Parse(node.Attributes.GetNamedItem("Eccentricity").Value);
                Inclination = Double.Parse(node.Attributes.GetNamedItem("Inclination").Value);
                ArgumentOfPeriapsis = Double.Parse(node.Attributes.GetNamedItem("ArgumentOfPeriapsis").Value);
                LongitudeOfAscendingNode = Double.Parse(node.Attributes.GetNamedItem("LongitudeOfAscendingNode").Value);
                MeanAnomolyAtEpoch = Double.Parse(node.Attributes.GetNamedItem("MeanAnomolyAtEpoch").Value);
                MeanDailyMotion = Double.Parse(node.Attributes.GetNamedItem("MeanDailyMotion").Value);
                Epoch = Double.Parse(node.Attributes.GetNamedItem("Epoch").Value);

            }
        }

        public void FromTLE(string line1, string line2, double gravity)
        {
            Epoch = SpaceTimeController.TwoLineDateToJulian(line1.Substr(18, 14));
            Eccentricity = double.Parse("0." + line2.Substr(26, 7));
            Inclination = double.Parse(line2.Substr(8, 8));
            LongitudeOfAscendingNode = double.Parse(line2.Substr(17, 8));
            ArgumentOfPeriapsis = double.Parse(line2.Substr(34, 8));
            double revs = double.Parse(line2.Substr(52, 11));
            MeanAnomolyAtEpoch = double.Parse(line2.Substr(43, 8));
            MeanDailyMotion = revs * 360.0;
            double part = (86400.0 / revs) / (Math.PI * 2.0);
            SemiMajorAxis = Math.Pow((part * part) * gravity, 1.0 / 3.0);
            SemiMajorAxisUnits = AltUnits.Meters;

        }
        public static bool IsTLECheckSumGood(string line)
        {
            if (line.Length != 69)
            {
                return false;
            }

            int checksum = 0;
            for (int i = 0; i < 68; i++ )
            {
                switch (line.Substr(i,1))
                {
                    case "1":
                        checksum += 1;
                        break;
                    case "2":
                        checksum += 2;
                        break;
                    case "3":
                        checksum += 3;
                        break;
                    case "4":
                        checksum += 4;
                        break;
                    case "5":
                        checksum += 5;
                        break;
                    case "6":
                        checksum += 6;
                        break;
                    case "7":
                        checksum += 7;
                        break;
                    case "8":
                        checksum += 8;
                        break;
                    case "9":
                        checksum += 9;
                        break;
                    case "-":
                        checksum += 1;

                        break;
                }
            }
            return (checksum % 10).ToString() == line.CharAt(68).ToString();

        }

        public string ToTLE()
        {
            //Epoch need to convert to TLE time string.
            // Ecentricity remove "0." from the begin and trim to 7 digits
            // Inclination decimal degrees 8 digits max
            // LOAN decimal degrees 8 digits
            // AOP
            // mean anomoly at epoch 8 digits
            // Mean motion (revs per day) Compute
            // Convert Semi-major-axis to meters from storage unit
            // Compute revs

            StringBuilder line1 = new StringBuilder();

            line1.Append("1 99999U 00111AAA ");
            line1.Append(SpaceTimeController.JulianToTwoLineDate(Epoch));
            line1.Append(" ");
            // line1.Append("-.00000001");
            line1.Append(SemiMajorAxis.ToExponential(4));
            line1.Append(" 00000-0 ");
            line1.Append(ToTLEExponential(MeanDailyMotion, 5));
            //line1.Append("-00000-1 0 ");
            line1.Append("  001");
            line1.Append(ComputeTLECheckSum(line1.ToString()));
            line1.AppendLine("");
            StringBuilder line2 = new StringBuilder();

            line2.Append("2 99999 ");
            line2.Append(TLENumberString(Inclination,3,4) + " ");
            line2.Append(TLENumberString(LongitudeOfAscendingNode, 3, 4) + " ");
            line2.Append((TLENumberString(Eccentricity, 1, 7) + " ").Substring(2));
            line2.Append(TLENumberString(ArgumentOfPeriapsis, 3, 4) + " ");
            line2.Append(TLENumberString(MeanAnomolyAtEpoch, 3, 4) + " ");
            line2.Append(ToTLEExponential(MeanDailyMotion / 207732, 5));
            line2.Append("00001");
            line2.Append(ComputeTLECheckSum(line2.ToString()));
            line2.AppendLine("");
            return line1.ToString() + line2.ToString();
        }

        public static string ToTLEExponential(double num, int size)
        {
            string exp = num.ToExponential(size);
            if (exp.Length < size+6)
            {
                exp = exp.Substring(0, size + 4) + "0" + exp.Substr(size + 4, 1);
            }
            return exp;
        }

        public static string TLENumberString(double num, int left, int right)
        {
            string formated = num.ToFixed(right);

            int point = formated.IndexOf(".");
            if (point == -1)
            {
                point = formated.Length;
                formated += ".0";
            }
            int len = formated.Length - point - 1;

            string fill = "00000000";

            formated = fill.Substr(0, left - point) + formated + fill.Substr(0, right - len);

            return formated;
        }

        public static char ComputeTLECheckSum(string line)
        {
            if (line.Length != 68)
            {
                return '0';
            }

            int checksum = 0;
            for (int i = 0; i < 68; i++)
            {
                switch (line[i])
                {
                    case '1':
                        checksum += 1;
                        break;
                    case '2':
                        checksum += 2;
                        break;
                    case '3':
                        checksum += 3;
                        break;
                    case '4':
                        checksum += 4;
                        break;
                    case '5':
                        checksum += 5;
                        break;
                    case '6':
                        checksum += 6;
                        break;
                    case '7':
                        checksum += 7;
                        break;
                    case '8':
                        checksum += 8;
                        break;
                    case '9':
                        checksum += 9;
                        break;
                    case '-':
                        checksum += 1;

                        break;
                }
            }
            return (char)( (char)(checksum % 10));
        }

        public EOE Elements
        {
            get
            {
                elements.a = SemiMajorAxis;
                elements.e = Eccentricity;
                elements.i = Inclination;
                //ArgumentOfPeriapsis += LongitudeOfAscendingNode;
                elements.w = ArgumentOfPeriapsis;
                elements.omega = LongitudeOfAscendingNode;
                elements.JDEquinox = Epoch;
                if (MeanDailyMotion == 0)
                {
                    elements.n = ELL.MeanMotionFromSemiMajorAxis(elements.a);
                }
                else
                {
                    elements.n = MeanDailyMotion;
                }
                elements.T = Epoch - (MeanAnomolyAtEpoch / elements.n);
                return elements;
            }
            set { elements = value; }
        }

        public void ComputeFrame(RenderContext renderContext)
        {
            switch (ReferenceFrameType)
            {
                case ReferenceFrameTypes.Orbital:
                    ComputeOrbital(renderContext);
                    break;
                case ReferenceFrameTypes.FixedSherical:
                    ComputeFixedSherical(renderContext);
                    break;
                case ReferenceFrameTypes.Trajectory:
                    ComputeFrameTrajectory(renderContext);
                    break;
    // todo port synodic for JWST orbits..
                //case ReferenceFrameTypes.FixedRectangular:
                //    ComputeFixedRectangular(renderContext);
                //    break;
                default:
                    break;
            }
        }

        public bool useRotatingParentFrame()
        {
            switch (ReferenceFrameType)
            {
                case ReferenceFrameTypes.Orbital:
                case ReferenceFrameTypes.Trajectory:
                case ReferenceFrameTypes.Synodic:
                    return false;
                default:
                    return true;
            }
        }

        private void ComputeFixedRectangular(RenderContext renderContext)
        {
            //WorldMatrix = Matrix3d.Identity;
            //WorldMatrix.Multiply(Matrix3d.RotationX(
            //WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)Heading, (float)Pitch, (float)Roll));
            //WorldMatrix.Translate((Vector3d)Translation);
            //WorldMatrix.Scale(new Vector3d(Scale, Scale, Scale));
            //WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)(Coordinates.MstFromUTC2(SpaceTimeController.Now, 0) / 180 * Math.PI), (float)0, (float)0));
        }
        private void ComputeFixedSherical(RenderContext renderContext)
        {
            if (ObservingLocation)
            {
                Lat = SpaceTimeController.Location.Lat;
                Lng = SpaceTimeController.Location.Lng;
                Altitude = SpaceTimeController.Altitude;
            }


            WorldMatrix = Matrix3d.Identity;
            WorldMatrix.Translate(Translation);
            double localScale = (1 / renderContext.NominalRadius) * Scale * MeanRadius;
            WorldMatrix.Scale( Vector3d.Create(localScale, localScale, localScale));
            //WorldMatrix.Scale(new Vector3d(1000, 1000, 1000));
            WorldMatrix.Multiply( Matrix3d.RotationYawPitchRoll((float)((Heading) / 180.0 * Math.PI), (float)(Pitch / 180.0 * Math.PI), (float)(Roll / 180.0 * Math.PI)));
            WorldMatrix.Multiply(Matrix3d.RotationZ(-90.0 / 180.0 * Math.PI));
            if (RotationalPeriod != 0)
            {
                double rotationCurrent = (((SpaceTimeController.JNow - this.ZeroRotationDate) / RotationalPeriod) * Math.PI * 2) % (Math.PI * 2);
                WorldMatrix.Multiply(Matrix3d.RotationX(-rotationCurrent));
            }
            WorldMatrix.Translate( Vector3d.Create(1 + (Altitude / renderContext.NominalRadius), 0, 0));
            WorldMatrix.Multiply(Matrix3d.RotationZ(Lat / 180 * Math.PI));
            WorldMatrix.Multiply(Matrix3d.RotationY(-(Lng+180) / 180 * Math.PI));



            ////  Vector3d offset = Coordinates.GeoTo3dDouble(Lat, Lng, 1 + (Altitude / renderContext.NominalRadius));
            //  WorldMatrix = Matrix3d.Identity;
            //  double localScale = (1 / renderContext.NominalRadius) * Scale * MeanRadius;
            //  WorldMatrix.Scale(new Vector3d(localScale, localScale, localScale));
            //  //WorldMatrix.Scale(new Vector3d(1000, 1000, 1000));
            //  WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)((Heading-90)/180.0*Math.PI), (float)Pitch, (float)Roll));
            //  WorldMatrix.Multiply(Matrix3d.RotationZ(-90.0));
            //  if (RotationalPeriod != 0)
            //  {
            //      double rotationCurrent = (((SpaceTimeController.JNow - this.ZeroRotationDate) / RotationalPeriod) * 360) % (360);
            //      WorldMatrix.Multiply(Matrix3d.RotationX(-rotationCurrent));
            //  }
            //  WorldMatrix.Translate(new Vector3d(1 + (Altitude / renderContext.NominalRadius), 0, 0));
            //  WorldMatrix.Multiply(Matrix3d.RotationZ(Lat));
            //  WorldMatrix.Multiply(Matrix3d.RotationY(-(Lng) ));
        }

        private void ComputeFrameTrajectory(RenderContext renderContext)
        {
            //Vector3d vector = new Vector3d();
            //Vector3d point = GetTragectoryPoint(SpaceTimeController.JNow, out vector);

            //Vector3d direction = vector;

            //direction.Normalize();
            //Vector3d up = point;
            //up.Normalize();
            //direction.Normalize();

            //double dist = point.Length();
            //double scaleFactor = 1.0;
            ////scaleFactor = UiTools.KilometersPerAu * 1000;
            //scaleFactor *= 1 / renderContext.NominalRadius;


            //WorldMatrix = Matrix3d.Identity;
            //Matrix3d look = Matrix3d.LookAtLH(new Vector3d(0, 0, 0), direction, new Vector3d(0,1,0));
            //look.Invert();

            //WorldMatrix = Matrix3d.Identity;


            //double localScale = (1 / renderContext.NominalRadius) * Scale * MeanRadius;
            //WorldMatrix.Scale(new Vector3d(localScale, localScale, localScale));
            //WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)Heading, (float)Pitch, (float)Roll));
            //if (RotationalPeriod != 0)
            //{
            //    double rotationCurrent = (((SpaceTimeController.JNow - this.ZeroRotationDate) / RotationalPeriod) * 360) % (360);
            //    WorldMatrix.Multiply(Matrix3d.RotationX(-rotationCurrent));
            //}

            //point = Vector3d.Scale(point, scaleFactor);

            //WorldMatrix.Translate(point);

            //if (StationKeeping)
            //{
            //    WorldMatrix = look * WorldMatrix;
            //}
        }

        //private Vector3d GetTragectoryPoint(double jNow, out Vector3d vector)
        //{
            //int min = 0;
            //int max = Trajectory.Count - 1;

            //Vector3d point =  new Vector3d();

            //vector = new Vector3d();

            //int current = max / 2;

            //bool found = false;

            //while (!found)
            //{
            //    if (current < 1)
            //    {
            //        vector = Trajectory[0].Position - Trajectory[1].Position;
            //        return Trajectory[0].Position;
            //    }


            //    if (current == Trajectory.Count - 1)
            //    {
            //        vector = Trajectory[current - 1].Position - Trajectory[current].Position;
            //        return Trajectory[current].Position;
            //    }

            //    if ((Trajectory[current-1].Time <= jNow) && (Trajectory[current].Time > jNow))
            //    {
            //        double denominator = Trajectory[current].Time -Trajectory[current-1].Time;
            //        double numerator = jNow - Trajectory[current - 1].Time;
            //        double tween = numerator / denominator;
            //        vector = Trajectory[current - 1].Position - Trajectory[current].Position;
            //        point = Vector3d.Lerp(Trajectory[current - 1].Position, Trajectory[current].Position, tween);
            //        return point;
            //    }

            //    if (Trajectory[current].Time < jNow)
            //    {
            //        int next = current + ( max - current + 1) / 2;
            //        min = current;
            //        current = next;
            //    }
            //    else
            //    if (Trajectory[current - 1].Time > jNow)
            //    {
            //        int next = current - ( current - min + 1) / 2;
            //        max = current;
            //        current = next;
            //    }
            //}

            //return point;
        //}

        private void ComputeOrbital(RenderContext renderContext)
        {
            EOE ee = Elements;
            Vector3d point = ELL.CalculateRectangularJD(SpaceTimeController.JNow, ee);
            MeanAnomoly = ee.meanAnnomolyOut;
            Vector3d pointInstantLater = ELL.CalculateRectangular(ee, MeanAnomoly+.001);

            Vector3d direction =  Vector3d.SubtractVectors(point, pointInstantLater);

            Vector3d up = point.Copy();
            up.Normalize();
            direction.Normalize();

            double dist = point.Length();
            double scaleFactor = 1.0;
            switch (SemiMajorAxisUnits)
            {
                case AltUnits.Meters:
                    scaleFactor = 1.0;
                    break;
                case AltUnits.Feet:
                    scaleFactor = 1.0 / 3.2808399;
                    break;
                case AltUnits.Inches:
                    scaleFactor = (1.0 / 3.2808399) / 12;
                    break;
                case AltUnits.Miles:
                    scaleFactor = 1609.344;
                    break;
                case AltUnits.Kilometers:
                    scaleFactor = 1000;
                    break;
                case AltUnits.AstronomicalUnits:
                    scaleFactor = UiTools.KilometersPerAu * 1000;
                    break;
                case AltUnits.LightYears:
                    scaleFactor = UiTools.AuPerLightYear * UiTools.KilometersPerAu * 1000;
                    break;
                case AltUnits.Parsecs:
                    scaleFactor = UiTools.AuPerParsec * UiTools.KilometersPerAu * 1000;
                    break;
                case AltUnits.MegaParsecs:
                    scaleFactor = UiTools.AuPerParsec * UiTools.KilometersPerAu * 1000 * 1000000;
                    break;
                case AltUnits.Custom:
                    scaleFactor = 1;
                    break;
                default:
                    break;
            }
            scaleFactor *= 1/renderContext.NominalRadius;


            Matrix3d look = Matrix3d.LookAtLH(Vector3d.Create(0,0,0), direction, up);
            look.Invert();

            WorldMatrix = Matrix3d.Identity;
            WorldMatrix.Translate(Translation);

            double localScale = (1 / renderContext.NominalRadius) * Scale * MeanRadius;
            WorldMatrix.Scale(Vector3d.Create(localScale, localScale, localScale));

            //Matrix3d mat = Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(Matrix3d.RotationY(Heading / 180.0 * Math.PI), Matrix3d.RotationX(Pitch / 180.0 * Math.PI)),Matrix3d.RotationZ(Roll / 180.0 * Math.PI));

            WorldMatrix.Multiply(Matrix3d.RotationYawPitchRoll((float)((Heading) / 180.0 * Math.PI), (float)(Pitch / 180.0 * Math.PI), (float)(Roll / 180.0 * Math.PI)));

            if (RotationalPeriod != 0)
            {
                double rotationCurrent = (((SpaceTimeController.JNow - this.ZeroRotationDate) / RotationalPeriod) * Math.PI * 2) % (Math.PI * 2);
                WorldMatrix.Multiply(Matrix3d.RotationX(-rotationCurrent));
            }

            point = Vector3d.Scale(point, scaleFactor);

            WorldMatrix.Translate(point);

            if (StationKeeping)
            {
                WorldMatrix = Matrix3d.MultiplyMatrix(look, WorldMatrix);
            }
        }
    }

    //public class TrajectorySample
    //{
    //    public double Time;
    //    public double X;
    //    public double Y;
    //    public double Z;
    //    public double H;
    //    public double P;
    //    public double R;
    //    public TrajectorySample(double time, double x, double y, double z)
    //    {
    //        X = x;
    //        Y = y;
    //        Z = z;
    //        Time = time;
    //    }

    //    public Vector3d Position
    //    {
    //        get
    //        {
    //            return Vector3d.Create(X*1000, Z*1000, Y*1000);
    //        }
    //    }

    //    public TrajectorySample(string line)
    //    {
    //        line = line.Replace("  ", " ");
    //        line = line.Replace("  ", " ");
    //        line = line.Replace("  ", " ");

    //        string[] parts = line.Split(new char[] { ' ', '\t', ',' });

    //        if (parts.Length > 3)
    //        {
    //            Time = double.Parse(parts[0]);
    //            X = double.Parse(parts[1]);
    //            Y = double.Parse(parts[2]);
    //            Z = double.Parse(parts[3]);
    //        }
    //        if (parts.Length > 6)
    //        {
    //            H = double.Parse(parts[4]);
    //            P = double.Parse(parts[5]);
    //            R = double.Parse(parts[6]);
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        if (H == 0 && P == 0 && R == 0)
    //        {
    //            return string.Format("{0} {1} {2} {3}", Time, X, Y, Z);
    //        }
    //        else
    //        {
    //            return string.Format("{0} {1} {2} {3} {4} {5} {6]", Time, X, Y, Z, H, P, R);
    //        }

    //    }
    //}
}
