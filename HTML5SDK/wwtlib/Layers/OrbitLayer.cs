using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Data.Files;

namespace wwtlib
{
    public class OrbitLayer : Layer
    {
        List<ReferenceFrame> frames = new List<ReferenceFrame>();

        public List<ReferenceFrame> Frames
        {
            get { return frames; }
            set { frames = value; }
        }

        OrbitLayerUI primaryUI = null;
        public override LayerUI GetPrimaryUI()
        {
            if (primaryUI == null)
            {
                primaryUI = new OrbitLayerUI(this);
            }

            return primaryUI;
        }

        public OrbitLayer()
        {
        }

        public override void CleanUp()
        {
            foreach (ReferenceFrame frame in frames)
            {
                if (frame.Orbit != null)
                {
                    frame.Orbit.CleanUp();
                    frame.Orbit = null;
                }
            }
        }

        public override void WriteLayerProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("PointOpacity", PointOpacity.ToString());
            xmlWriter.WriteAttributeString("PointColor", pointColor.Save());

        }

        private double pointOpacity = 1;


        public double PointOpacity
        {
            get { return pointOpacity; }
            set
            {
                if (pointOpacity != value)
                {
                    version++;

                    pointOpacity = value;

                }
            }
        }

        Color pointColor = Colors.Yellow;


        public Color PointColor
        {
            get { return pointColor; }
            set
            {
                if (pointColor != value)
                {
                    version++;
                    pointColor = value;

                }
            }
        }

        public override double[] GetParams()
        {
            double[] paramList = new double[6];
            paramList[0] = pointOpacity;
            paramList[1] = Color.R / 255;
            paramList[2] = Color.G / 255;
            paramList[3] = Color.B / 255;
            paramList[4] = Color.A / 255;
            paramList[5] = Opacity;


            return paramList;
        }

        public override string[] GetParamNames()
        {
            return new string[] { "PointOpacity", "Color.Red", "Color.Green", "Color.Blue", "Color.Alpha", "Opacity" };
        }

        //public override BaseTweenType[] GetParamTypes()
        //{
        //    return new BaseTweenType[] { BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear };
        //}

        public override void SetParams(double[] paramList)
        {
            if (paramList.Length == 6)
            {
                pointOpacity = paramList[0];
                Opacity = (float)paramList[5];
                Color color = Color.FromArgb((int)(paramList[4] * 255), (int)(paramList[1] * 255), (int)(paramList[2] * 255), (int)(paramList[3] * 255));
                Color = color;

            }
        }


        public override void InitializeFromXml(XmlNode node)
        {
            PointOpacity = double.Parse(node.Attributes.GetNamedItem("PointOpacity").Value);
            PointColor = Color.Load(node.Attributes.GetNamedItem("PointColor").Value);

        }


        public override bool Draw(RenderContext renderContext, float opacity, bool flat)
        {
            Matrix3d matSaved = renderContext.World;
            renderContext.World = renderContext.WorldBaseNonRotating;

            foreach (ReferenceFrame frame in frames)
            {
                if (frame.ShowOrbitPath)
                {
                    if (frame.Orbit == null)
                    {
                        frame.Orbit = new Orbit(frame.Elements, 360, this.Color, 1, (float)renderContext.NominalRadius);
                    }
                    frame.Orbit.Draw3D(renderContext, opacity * this.Opacity, new Vector3d());
                }
            }
            renderContext.World = matSaved;
            return true;
        }

        string filename = "";

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            filename = fc.TempDirectory + string.Format("{0}\\{1}.txt", fc.PackageID, this.ID.ToString());

            string dir = filename.Substring(0, filename.LastIndexOf("\\"));

            Blob blob = new Blob(new object[] { dataFile });

            fc.AddFile(filename, blob);

            base.AddFilesToCabinet(fc);

        }

        string dataFile = "";

        public override void LoadData(TourDocument tourDoc, string filename)
        {
            Blob blob = tourDoc.GetFileBlob(filename);
            FileReader doc = new FileReader();
            doc.OnLoadEnd = delegate (FileProgressEvent ee)
            {
                dataFile = doc.Result as string;
                LoadString(dataFile);

            };
            doc.ReadAsText(blob);
        }

        public void LoadString(string dataFile)
        {
                string[] data = dataFile.Split("\n");
                frames.Clear();
                for (int i = 0; i < data.Length; i += 2)
                {
                    int line1 = i;
                    int line2 = i + 1;
                    if (data[i].Length > 0)
                    {
                        ReferenceFrame frame = new ReferenceFrame();
                        if (data[i].Substring(0, 1) != "1")
                        {
                            line1++;
                            line2++;
                            frame.Name = data[i].Trim();
                            i++;
                        }
                        else if (data[i].Substring(0, 1) == "1")
                        {
                            frame.Name = data[i].Substring(2, 5);
                        }
                        else
                        {
                            i -= 2;
                            continue;
                        }

                        frame.Reference = ReferenceFrames.Custom;
                        frame.Oblateness = 0;
                        frame.ShowOrbitPath = true;
                        frame.ShowAsPoint = true;
                        frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
                        frame.Scale = 1;
                        frame.SemiMajorAxisUnits = AltUnits.Meters;
                        frame.MeanRadius = 10;
                        frame.Oblateness = 0;
                        frame.FromTLE(data[line1], data[line2], 398600441800000);
                        frames.Add(frame);
                    }
                    else
                    {
                        i -= 1;
                    }
                }
        }
    }



    public class OrbitLayerUI : LayerUI
    {
        OrbitLayer layer = null;
        bool opened = true;

        public OrbitLayerUI(OrbitLayer layer)
        {
            this.layer = layer;
        }
        IUIServicesCallbacks callbacks = null;

        public override void SetUICallbacks(IUIServicesCallbacks callbacks)
        {
            this.callbacks = callbacks;
        }
        public override bool HasTreeViewNodes
        {
            get
            {
                return true;
            }
        }

        public override List<LayerUITreeNode> GetTreeNodes()
        {
            List<LayerUITreeNode> nodes = new List<LayerUITreeNode>();
            foreach (ReferenceFrame frame in layer.Frames)
            {

                LayerUITreeNode node = new LayerUITreeNode();
                node.Name = frame.Name;


                node.Tag = frame;
                node.Checked = frame.ShowOrbitPath;
                node.NodeSelected += new LayerUITreeNodeSelectedDelegate(node_NodeSelected);
                node.NodeChecked += new LayerUITreeNodeCheckedDelegate(node_NodeChecked);
                nodes.Add(node);
            }
            return nodes;
        }

        void node_NodeChecked(LayerUITreeNode node, bool newState)
        {
            ReferenceFrame frame = (ReferenceFrame)node.Tag;

            if (frame != null)
            {
                frame.ShowOrbitPath = newState;
            }
        }



        void node_NodeSelected(LayerUITreeNode node)
        {
            if (callbacks != null)
            {
                ReferenceFrame frame = (ReferenceFrame)node.Tag;

                Dictionary<String, String> rowData = new Dictionary<string, string>();

                rowData["Name"] = frame.Name;
                rowData["SemiMajor Axis"] = frame.SemiMajorAxis.ToString();
                rowData["SMA Units"] = frame.SemiMajorAxisUnits.ToString();
                rowData["Inclination"] = frame.Inclination.ToString();
                rowData["Eccentricity"] = frame.Eccentricity.ToString();
                rowData["Long of Asc. Node"] = frame.LongitudeOfAscendingNode.ToString();
                rowData["Argument Of Periapsis"] = frame.ArgumentOfPeriapsis.ToString();
                rowData["Epoch"] = frame.Epoch.ToString();
                rowData["Mean Daily Motion"] = frame.MeanDailyMotion.ToString();
                rowData["Mean Anomoly at Epoch"] = frame.MeanAnomolyAtEpoch.ToString();
                callbacks.ShowRowData(rowData);
            }
        }

        public override List<LayerUIMenuItem> GetNodeContextMenu(LayerUITreeNode node)
        {
            return base.GetNodeContextMenu(node);
        }
    }
}
