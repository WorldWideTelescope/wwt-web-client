using System;
namespace wwtlib
{
    interface IFolder
    {
        bool Browseable { get; set; }
        System.Collections.Generic.List<Folder> Folders { get; set; }
        FolderGroup Group { get; set; }
        System.Collections.Generic.List<Imageset> Imagesets { get; set; }
        long MSRCommunityId { get; set; }
        long MSRComponentId { get; set; }
        string Name { get; set; }
        long Permission { get; set; }
        System.Collections.Generic.List<Place> Places { get; set; }
        bool ReadOnly { get; set; }
        string RefreshInterval { get; set; }
        FolderRefreshType RefreshType { get; set; }
        bool RefreshTypeSpecified { get; set; }
        bool Searchable { get; set; }
        string SubType { get; set; }
        string ThumbnailUrl { get; set; }
        System.Collections.Generic.List<Tour> Tours { get; set; }
        FolderType Type { get; set; }
        string Url { get; set; }
    }
}
