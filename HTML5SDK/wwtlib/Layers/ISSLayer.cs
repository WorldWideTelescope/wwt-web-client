using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Data.Files;

namespace wwtlib
{
    delegate void BackInitDelegate();
    public class ISSLayer : Object3dLayer
    {
        public ISSLayer()
        {
            ID = ISSGuid;
        }

        public static Guid ISSGuid =  Guid.FromString("00000001-0002-0003-0405-060708090a0b");

        public override bool Draw(RenderContext renderContext, float opacity, bool flat)
        {
            if (object3d == null && issmodel == null)
            {
                if (!loading)
                {
                    Matrix3d worldView = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);
                    Vector3d v = worldView.Transform(Vector3d.Empty);
                    double scaleFactor = Math.Sqrt(worldView.M11 * worldView.M11 + worldView.M22 * worldView.M22 + worldView.M33 * worldView.M33);
                    double dist = v.Length();
                    double radius = scaleFactor;

                    // Calculate pixelsPerUnit which is the number of pixels covered
                    // by an object 1 AU at the distance of the planet center from
                    // the camera. This calculation works regardless of the projection
                    // type.
                    int viewportHeight = (int)renderContext.Height;
                    double p11 = renderContext.Projection.M11;
                    double p34 = renderContext.Projection.M34;
                    double p44 = renderContext.Projection.M44;


                    double w = Math.Abs(p34) * dist + p44;
                    float pixelsPerUnit = (float)(p11 / w) * viewportHeight;
                    float radiusInPixels = (float)(radius * pixelsPerUnit);
                    if (radiusInPixels > 0.5f)
                    {
                        LoadBackground();
                    }
                }

            }

            object3d = issmodel;
            return base.Draw(renderContext, opacity, flat);
        }


        //todo need to synce the ISS settings to settings.active thru keeping copy of last setting & enabled state and setting the other to match when changed.
        //public override bool Enabled
        //{
        //    get
        //    {
        //        return base.Enabled = Settings.Active.ShowISSModel;
        //    }
        //    set
        //    {
        //        Properties.Settings.Default.ShowISSModel = base.Enabled = value;
        //    }
        //}

        public override LayerUI GetPrimaryUI()
        {
            return null;
        }

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            return;
        }

        public override void LoadData(TourDocument doc, string filename)
        {
            return;
        }
        public override void CleanUp()
        {
           // base.CleanUp();
        }

        static bool loading = false;

        static Object3d issmodel = null;
        static TourDocument doc = null;

        public static void LoadBackground()
        {
            if (loading)
            {
                return;
            }

            loading = true;
            string url = "http://www.worldwidetelescope.org/data/iss.wtt";

            doc = TourDocument.FromUrlRaw(url,
                delegate
                {
                    CreateSpaceStation();
                });
        }

        public static void CreateSpaceStation()
        {
            doc.Id = "28016047-97a9-4b33-a226-cd820262a151";
            string filename = "0c10ae54-b6da-4282-bfda-f34562d403bc.3ds";

            Object3d o3d = new Object3d(doc, filename, true, false, true, Colors.White);
            if (o3d != null)
            {
                o3d.ISSLayer = true;
                issmodel = o3d;
            }
        }
    }
}
