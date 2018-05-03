using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;


namespace wwtlib
{
    class FolderUp : IThumbnail
    {
        public FolderUp()
        {
        }

        #region IThumbnail Members

        public string Name
        {
            get { return "Up Level"; }
        }


        public Folder Parent = null;

        ImageElement thumbnail;
        public System.Html.ImageElement Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                thumbnail = value; ;
            }
        }

        public string ThumbnailUrl
        {
            get
            {
                return "//worldwidetelescope.org/wwtweb/thumbnail.aspx?Name=folderup";
            }
            set
            {
                return;
            }
        }
        Rectangle bounds = new Rectangle();
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        public bool IsImage
        {
            get { return false; }
        }

        public bool IsTour
        {
            get { return false; }
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
            get
            {
                if (Parent == null)
                {
                    return new List<IThumbnail>();
                }
                else
                {
                    return Parent.Children;
                }
            }
        }
        #endregion
    }
}
