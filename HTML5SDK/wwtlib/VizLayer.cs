using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class VizLayer
    {
        public VizLayer()
        {
        }

        public List<String[]> table = new List<string[]>();
        public string[] header;

        public List<DataItem> items = new List<DataItem>();

        ImageElement starProfile;
        bool imageReady = false;
        public void Load(string data)
        {
            string[] lines = data.Split("\r\n");

            starProfile = (ImageElement)Document.CreateElement("img");
            starProfile.AddEventListener("load", delegate(ElementEvent e)
            {
                imageReady = true;
            }, false);
            starProfile.Src = "images/StarProfileAlpha.png";
            bool gotHeader = false;
            foreach (string line in lines)
            {
                if (gotHeader)
                {
                    table.Add(line.Split("\t"));
                }
                else
                {
                    header = line.Split("\t");
                    gotHeader = true;
                }
            }
        }

        int DateColumn = 0;
        int latColumn = 1;
        int lngColumn = 2;
        int depthColumn = 3;
        int magColumn = 4;

        public const double earthRadius = 6371000;

        Vector3d[] transformedList;
        Vector3d[] worldList;
        
        public void Prepare()
        {
            worldList = new Vector3d[table.Count];
            transformedList = new Vector3d[table.Count];

            int index = 0;
            foreach (string[] row in table)
            {
                DataItem item = new DataItem();
                item.EventTime = Date.Parse(row[DateColumn]);
                double radius = (earthRadius - double.Parse(row[depthColumn])*1000)/ earthRadius;
                item.Location = Coordinates.GeoTo3dRad(double.Parse(row[latColumn]), double.Parse(row[lngColumn])+180, radius);
                item.Tranformed = new Vector3d();
                item.Size = (float)Math.Pow(2, double.Parse(row[magColumn]))/50;

                worldList[index] = item.Location;
                transformedList[index] = item.Tranformed;
                items.Add(item);
                index++;
            }
        }

        public void Draw(RenderContext renderContext)
        {
            if (!imageReady)
            {
                return;
            }

            renderContext.Device.Save();

            renderContext.WVP.ProjectArrayToScreen(worldList, transformedList);
            CanvasContext2D ctx = renderContext.Device;
            ctx.Alpha = .4;

            double width = renderContext.Width;
            double height = renderContext.Height;

            Vector3d viewPoint = Vector3d.MakeCopy(renderContext.ViewPoint);

            double scaleFactor = renderContext.FovScale/100;
            foreach (DataItem item in items)
            {
               // if (Vector3d.Dot(viewPoint, item.Location) < 0)
                if (item.Tranformed.Z < 1)
                {
                    double x = item.Tranformed.X;
                    double y = item.Tranformed.Y;
                    double size = 4*item.Size / scaleFactor;
                    double half = size / 2;
                    if (x > -half && x < width + half && y > -half && y < height + half)
                    {
                        ctx.DrawImage(starProfile, x - size / 2, y - size / 2, size, size);

                        //ctx.BeginPath();
                        //ctx.FillStyle = "rgb(200,0,0)";
                        //ctx.Arc(x, y, size, 0, Math.PI * 2, true);
                        //ctx.Fill();
                    }
                }

            }

            renderContext.Device.Restore();
            
        }
    }

    public class DataItem
    {
        public DataItem()
        {
        }

        public Date EventTime;
        public Vector3d Location;
        public Vector3d Tranformed;
        public Color Color;
        public double Size;
        public String GetColor()
        {
            return "Red";
        }
    
    }


}
