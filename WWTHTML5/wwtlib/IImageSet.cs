using System;
using System.Collections.Generic;
using System.Linq;

namespace wwtlib
{
    //public interface IImageSet
    //{
    //    string DemUrl { get; set; }
    //    BandPass BandPass { get; set; }
    //    int BaseLevel { get; set; }
    //    double BaseTileDegrees { get; set; }
    //    bool BottomsUp { get; set; }
    //    double CenterX { get; set; }
    //    double CenterY { get; set; }
    //    string CreditsText { get; set; }
    //    string CreditsUrl { get; set; }
    //    ImageSetType DataSetType { get; set; }
    //    bool DefaultSet { get; set; }
    //    bool ElevationModel { get; set; }
    //    string Extension { get; set; }
    //    bool Generic { get; set; }
    //    int GetHashCode();
    //    int ImageSetID { get; set; }
    //    bool IsMandelbrot { get; }
    //    int Levels { get; set; }
    //    Matrix3d Matrix { get; set; }
    //    bool Mercator { get; set; }
    //    string Name { get; set; }
    //    double OffsetX { get; set; }
    //    double OffsetY { get; set; }
    //    ProjectionType Projection { get; set; }
    //    string QuadTreeTileMap { get; set; }
    //    double Rotation { get; set; }
    //    bool Sparse { get; set; }
    //    IImageSet StockImageSet { get; }
    //    string ThumbnailUrl { get; set; }
    //    string Url { get; set; }
    //    double WidthFactor { get; set; }
    //    //        WcsImage WcsImage { get; set; }
    //    double MeanRadius { get; set; }
    //    string ReferenceFrame { get; set; }
    //}

    public enum ProjectionType { Mercator = 0, Equirectangular = 1, Tangent = 2, Tan = 2, Toast = 3, Spherical = 4, SkyImage = 5, Plotted = 6 };
    public enum ImageSetType { Earth = 0, Planet = 1, Sky = 2, Panorama = 3, SolarSystem = 4, Sandbox = 5};
    public enum BandPass { Gamma = 0, XRay = 1, Ultraviolet = 2, Visible = 3, HydrogenAlpha = 4, IR = 4, Microwave = 5, Radio = 6, VisibleNight = 6 };
}
