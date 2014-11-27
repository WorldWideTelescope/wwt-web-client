using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Net;

namespace wwtlib
{
    class Wtml
    {
        static public void GetWtmlFile(string url, Action complete)
        {
            Folder temp = new Folder();
            temp.LoadFromUrl(url, delegate { LoadImagesets(temp); complete(); });
        }

        static public void LoadImagesets(Folder folder)
        {

            foreach (object child in folder.Children)
            {
                Imageset imageset = null;

                if (child is Imageset)
                {
                    imageset = (Imageset)child;
                    WWTControl.ImageSets.Add(imageset);
                }
                if (child is Place)
                {
                    Place place = (Place)child;
                    if (place.StudyImageset != null)
                    {
                        WWTControl.ImageSets.Add(place.StudyImageset);
                    }
                    
                    if (place.BackgroundImageset != null)
                    {
                        WWTControl.ImageSets.Add(place.BackgroundImageset);
                    }
                }
            }
                   

            if (!string.IsNullOrEmpty(WWTControl.ImageSetName))
            {
                string name = WWTControl.ImageSetName.ToLowerCase();
                foreach (Imageset imageset in WWTControl.ImageSets)
                {
                    if (imageset.Name.ToLowerCase() == name)
                    {
                        WWTControl.Singleton.RenderContext.BackgroundImageset = imageset;
                    }
                }
            }

        }

        //static public void GetWtmlFile(string url, Action complete)
        //{
        //    XmlHttpRequest xhr = new XmlHttpRequest();
        //    xhr.Open(HttpVerb.Get, url);
        //    xhr.OnReadyStateChange = delegate()
        //    {
        //        if (xhr.ReadyState == ReadyState.Loaded)
        //        {
        //            ParseWtmlFile(xhr.ResponseXml);
        //            complete();
        //        }
        //    };
        //    xhr.Send();
        //}
        //static public void ParseWtmlFile(XmlDocument doc)
        //{
        //    XmlNode node = Util.SelectSingleNode(doc, "Folder");


        //    foreach (XmlNode child in node.ChildNodes)
        //    {
        //        if (child.Attributes != null)
        //        {
        //            Imageset ish = Imageset.FromXMLNode(child);
        //            //Place place = Place.Create(ish.Name, ish.CenterX, ish.CenterY, Classification.Star, "UMA", ish.DataSetType, 1);
        //            //place.BackgroundImageSet = ish;
        //            WWTControl.ImageSets.Add(ish);
        //        }
        //    }

        //    if (!string.IsNullOrEmpty(WWTControl.ImageSetName))
        //    {
        //        string name = WWTControl.ImageSetName.ToLowerCase();
        //        foreach (Imageset imageset in WWTControl.ImageSets)
        //        {
        //            if (imageset.Name.ToLowerCase() == name)
        //            {
        //                WWTControl.Singleton.RenderContext.BackgroundImageset = imageset;
        //            }
        //        }
        //    }

        //}
    }
}
