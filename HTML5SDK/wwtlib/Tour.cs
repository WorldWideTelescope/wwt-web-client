using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;


namespace wwtlib
{
    public class Tour : IThumbnail
    {
        public Tour()
        {
        }

        internal static Tour FromXml(System.Xml.XmlNode child)
        {
            Tour temp = new Tour();
            if (child.Attributes.GetNamedItem("ID") != null)
            {
                temp.Id = child.Attributes.GetNamedItem("ID").Value;
            }
            if (child.Attributes.GetNamedItem("TourUrl") != null)
            {
                temp.tourUrl = child.Attributes.GetNamedItem("TourUrl").Value;
            }

            if (child.Attributes.GetNamedItem("Title") != null)
            {
               temp.Title = child.Attributes.GetNamedItem("Title").Value;
            }

            if (child.Attributes.GetNamedItem("Description") != null)
            {
               temp.Description = child.Attributes.GetNamedItem("Description").Value;
            }
            if (child.Attributes.GetNamedItem("Classification") != null)
            {
                temp.Classification = (Classification)Enums.Parse("Classification", child.Attributes.GetNamedItem("Classification").Value);
            }
            if (child.Attributes.GetNamedItem("AuthorEmail") != null)
            {
               temp.AuthorEmail = child.Attributes.GetNamedItem("AuthorEmail").Value;
            }
            if (child.Attributes.GetNamedItem("Author") != null)
            {
               temp.Author = child.Attributes.GetNamedItem("Author").Value;
            }
            if (child.Attributes.GetNamedItem("AuthorURL") != null)
            {
               temp.AuthorURL = child.Attributes.GetNamedItem("AuthorURL").Value;
            }

            if (child.Attributes.GetNamedItem("AuthorImageUrl") != null)
            {
                temp.AuthorImageUrl = child.Attributes.GetNamedItem("AuthorImageUrl").Value;
            }

            if (child.Attributes.GetNamedItem("AverageRating") != null)
            {
               temp.AverageRating = double.Parse(child.Attributes.GetNamedItem("AverageRating").Value);
            }
            if (child.Attributes.GetNamedItem("LengthInSecs") != null)
            {
               temp.LengthInSecs = double.Parse(child.Attributes.GetNamedItem("LengthInSecs").Value);
            }
            if (child.Attributes.GetNamedItem("OrganizationUrl") != null)
            {
              temp.OrganizationUrl  = child.Attributes.GetNamedItem("OrganizationUrl").Value;
            }
            if (child.Attributes.GetNamedItem("OrganizationName") != null)
            {
               temp.OrganizationName = child.Attributes.GetNamedItem("OrganizationName").Value;
            }
            if (child.Attributes.GetNamedItem("RelatedTours") != null)
            {
               temp.RelatedTours = child.Attributes.GetNamedItem("RelatedTours").Value;
            }
            if (child.Attributes.GetNamedItem("Keywords") != null)
            {
               temp.Keywords = child.Attributes.GetNamedItem("Keywords").Value;
            }
            if (child.Attributes.GetNamedItem("ThumbnailUrl") != null)
            {
                temp.ThumbnailUrl = child.Attributes.GetNamedItem("ThumbnailUrl").Value;
            }

            return temp;

        }

        public UserLevel userLevel;
        public string Title;
        public string Taxonomy;
        public string OrganizationName;
        public string OrganizationUrl;
        public string Id;
        public string Description;
        public Classification Classification;
        public string AuthorEmail;
        public string Author;
        public string TourDuration;
        public string AuthorURL;
        public double AverageRating;
        public double LengthInSecs;
        public string TourAttributionAndCredits;
        public string AuthorImageUrl;
        public string Keywords;
        public string RelatedTours;


        #region IThumbnail Members

        public string Name
        {
            get { return Title; }
        }

        ImageElement thumbnail;
        public System.Html.ImageElement Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                thumbnail = value;
            }
        }
        string thumbnailUrlField = "";
        public string ThumbnailUrl
        {
            get
            {
                if (!String.IsNullOrEmpty(thumbnailUrlField))
                {
                    return thumbnailUrlField;
                }
                else
                {
                    return String.Format("http://cdn.worldwidetelescope.org/wwtweb/GetTourThumbnail.aspx?GUID={0}", Id);
                }
            }
            set
            {

                thumbnailUrlField = value.ToString();
            }
        }
        string tourUrl;
        public string TourUrl
        {
            get
            {
                if (string.IsNullOrEmpty(tourUrl))
                {
                    return string.Format("http://cdn.worldwidetelescope.org/wwtweb/GetTour.aspx?GUID={0}", this.Id);
                }
                else
                {
                    return tourUrl;
                }
            }
            set { tourUrl = value; }
        }


        Rectangle bounds;
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds=value;
            }
        }

        public bool IsImage
        {
            get { return false; }
        }

        public bool IsTour
        {
            get { return true; }
        }

        public bool IsFolder
        {
            get { return false; }
        }

        public bool IsCloudCommunityItem
        {
            get { return false; }
        }

        public bool ReadOnly
        {
            get { return false; }
        }

        public List<IThumbnail> Children
        {
            get { return new List<IThumbnail>();}
        }

        #endregion
    }
}
