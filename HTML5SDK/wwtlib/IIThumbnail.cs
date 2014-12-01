using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;

namespace wwtlib
{
    public interface IThumbnail
    {
        string Name { get; }
        ImageElement Thumbnail { get; set; }
        string ThumbnailUrl { get; set; }
        Rectangle Bounds { get; set; }
        bool IsImage { get; }
        bool IsTour { get; }
        bool IsFolder { get; }
        bool IsCloudCommunityItem { get; }
        bool ReadOnly { get; }
        List<IThumbnail> Children { get; }
    }

    
}
