using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class FolderBrowser
    {
        public FolderBrowser()
        {
        }

        public CanvasElement Canvas;
        List<IThumbnail> items = new List<IThumbnail>();

        public static FolderBrowser Create()
        {
            FolderBrowser temp = new FolderBrowser();

            temp.Height = 85;
            temp.Width = 1920;
            temp.Canvas = (CanvasElement)Document.CreateElement("canvas");
            temp.Canvas.Width = temp.Width;
            temp.Canvas.Height = temp.Height;
            //temp.Canvas.Style.MarginBottom = "0";
            temp.Setup();
            temp.LoadImages();

            return temp;
        }

        public int Top = 10;
        public int Left = 10;

        public void Setup()
        {
            Canvas.AddEventListener("click", OnClick, false);
            Canvas.AddEventListener("dblclick", OnDoubleClick, false);
            Canvas.AddEventListener("mousemove", OnMouseMove, false);
            Canvas.AddEventListener("mouseup", OnMouseUp, false);
            Canvas.AddEventListener("mousedown", OnMouseDown, false);
            Canvas.AddEventListener("mousewheel", OnMouseWheel, false);
            Canvas.AddEventListener("touchstart", OnTouchStart, false);
            Canvas.AddEventListener("touchmove", OnTouchMove, false);
            Canvas.AddEventListener("touchend", OnTouchEnd, false);
            //Document.Body.AddEventListener("gesturechange", OnGestureChange, false);
            Canvas.AddEventListener("mouseout", OnMouseUp, false);

        }
   

        int indexTouchDown = -1;
        public void OnTouchStart(ElementEvent e)
        {
            TouchEvent ev = (TouchEvent)e;
            ev.PreventDefault();
            mouseDown = true;
            lastX = ev.TargetTouches[0].PageX;
            lastY = ev.TargetTouches[0].PageY;

            indexTouchDown = GetItemIndexFromCursor(Vector2d.Create(ev.TargetTouches[0].PageX, ev.TargetTouches[0].PageY));
           
        }

        public void OnTouchMove(ElementEvent e)
        {
            TouchEvent ev = (TouchEvent)e;
            ev.PreventDefault();
            if (mouseDown)
            {
                double curX = ev.TargetTouches[0].PageX - lastX;

                double curY = ev.TargetTouches[0].PageY - lastY;
                if (mouseDown)
                {
                    dragging = true;
                }

                if (!dragging)
                {
                    int newHover = GetItemIndexFromCursor(Vector2d.Create(ev.TargetTouches[0].PageX, ev.TargetTouches[0].PageY));
                    if (hoverItem != newHover)
                    {
                        hoverItem = newHover;
                        //if (ItemHover != null)
                        //{
                        //    if (hoverItem > -1)
                        //    {
                        //        ItemHover.Invoke(this, items[hoverItem]);
                        //    }
                        //    else
                        //    {
                        //        ItemHover.Invoke(this, null);
                        //    }
                        //}
                    }
                }
                else
                {
                    int tiles = (int)Math.Round(((ev.TargetTouches[0].PageX - lastX) + startOffset) / HorzSpacing);
                    int offset = (int)Math.Round(((ev.TargetTouches[0].PageX - lastX) + startOffset) - (tiles * HorzSpacing));

                    startOffset = offset;
                    startIndex -= tiles;
                    if (startIndex < 0)
                    {
                        startOffset -= (HorzSpacing * startIndex);
                        startIndex = 0;
                    }
                    lastX = ev.TargetTouches[0].PageX;
                    lastY = ev.TargetTouches[0].PageY;
                }
                Refresh();
              
               
            }
        }

        public void OnTouchEnd(ElementEvent e)
        {
           
            TouchEvent ev = (TouchEvent)e;
            ev.PreventDefault();
            if (dragging)
            {
                dragging = false;
                ignoreClick = true;
            }
            else if (indexTouchDown > -1 && mouseDown)
            {
                HandleClick(indexTouchDown);
            }
            startOffset = 0;
            mouseDown = false;
            Refresh();
        }


        public void OnClick(ElementEvent e)
        {
            if (!ignoreClick)
            {
                int index = GetItemIndexFromCursor(Vector2d.Create(e.OffsetX, e.OffsetY));
                HandleClick(index);
            
            }
            else
            {
                ignoreClick = false;
            }
        }

        private void HandleClick(int index)
        {
            if (index > -1)
            {
                if (items[index] is Place)
                {
                    //Place place = (Place)items[index];
                    //if (place.BackgroundImageSet != null)
                    //{
                    //    WWTControl.Singleton.RenderContext.BackgroundImageset = place.BackgroundImageSet;
                    //}

                    //if (place.StudyImageset != null)
                    //{
                    //    //  WWTControl.Singleton.RenderContext.ForegroundImageSet = place.StudyImageset;
                    //    WWTControl.Singleton.RenderContext.ForegroundImageset = place.StudyImageset;


                    //}
                    //if (!(place.Lat == 0 && place.Lng == 0))
                    //{
                    //    WWTControl.Singleton.RenderContext.ViewCamera = place.CamParams;
                    //}
                    Place place = (Place)items[index];
                   
                    WWTControl.Singleton.GotoTarget(place, false, false, true);
                  


                    return;
                }

                if (items[index] is Imageset)
                {
                    Imageset imageset = (Imageset)items[index];

                    WWTControl.Singleton.RenderContext.BackgroundImageset = imageset;
                    return;


                }

                if (items[index] is Tour)
                {
                    Tour tour = (Tour)items[index];

                    WWTControl.Singleton.PlayTour(tour.TourUrl);
                    return;


                }


                if (items[index] is Folder)
                {
                    Folder folder = (Folder)items[index];
                    startIndex = 0;
                    folder.ChildLoadCallback(delegate { items = folder.Children; Refresh(); });
                    return;
                }

                if (items[index] is FolderUp)
                {
                    FolderUp folderUp = (FolderUp)items[index];
                    if (folderUp.Parent != null)
                    {
                        startIndex = 0;
                        folderUp.Parent.ChildLoadCallback(delegate { items = folderUp.Parent.Children; Refresh(); });
                    }
                    return;
                }
            }
            return;
        }

        public void OnDoubleClick(ElementEvent e)
        {
            RenderTriangle.RenderingOn = !RenderTriangle.RenderingOn;
        } 
        
        public void OnGestureChange(ElementEvent e)
        {
            GestureEvent g = (GestureEvent)e;
            mouseDown = false;
            double delta = g.Scale;

      //      if (delta > 1 && Math.Abs(delta - 1) > .05)

        }


       

        bool mouseDown = false;
        double lastX;
        double lastY;
        public void OnMouseDown(ElementEvent e)
        {
            mouseDown = true;
            lastX = Mouse.OffsetX(Canvas, e);
            lastY = Mouse.OffsetY(Canvas, e);
        }

        public void OnMouseMove(ElementEvent e)
        {

            if (mouseDown)
            {
                dragging = true;
            }

            if (!dragging)
            {
                int newHover = GetItemIndexFromCursor(Vector2d.Create(Mouse.OffsetX(Canvas, e), Mouse.OffsetY(Canvas, e)));
                if (hoverItem != newHover)
                {
                    hoverItem = newHover;
                    //if (ItemHover != null)
                    //{
                    //    if (hoverItem > -1)
                    //    {
                    //        ItemHover.Invoke(this, items[hoverItem]);
                    //    }
                    //    else
                    //    {
                    //        ItemHover.Invoke(this, null);
                    //    }
                    //}
                }
            }
            else
            {
                int tiles = (int)Math.Round(((Mouse.OffsetX(Canvas, e) - lastX) + startOffset) / HorzSpacing);
                int offset = (int)Math.Round(((Mouse.OffsetX(Canvas, e) - lastX) + startOffset) - (tiles * HorzSpacing));

                startOffset = offset;
                startIndex -= tiles;
                if (startIndex < 0)
                {
                    startOffset -= (HorzSpacing * startIndex);
                   startIndex = 0;
                }
                lastX = Mouse.OffsetX(Canvas, e);
                lastY = Mouse.OffsetY(Canvas, e);
            }
            Refresh();
           
        }

        bool ignoreClick = false;
        public void OnMouseUp(ElementEvent e)
        {
            if (dragging)
            {
                //startIndex = (int)Math.Round(Math.Max(0, (int)startIndex - startOffset / horzMultiple));
                startOffset = 0;
                dragging = false;
                ignoreClick = true;
            }


            mouseDown = false;
            Refresh();

        }

        public void OnMouseWheel(ElementEvent e)
        {
            WheelEvent ev = (WheelEvent)(object)e;
            //firefox
            //    double delta = event.detail ? event.detail * (-120) : event.wheelDelta;
            double delta = ev.WheelDelta;

        }
        //int imageCount = 0;
        //int imageLoadCount = 0;
        //bool imagesLoaded = false;
        //bool downloading = false;
        //ImageElement LoadImageElement(string url)
        //{
        //    imageCount++;
        //    imagesLoaded = false;
        //    downloading = true;
        //    ImageElement temp = (ImageElement)Document.CreateElement("img");
        //    temp.Src = url;
        //    temp.AddEventListener("load", delegate(ElementEvent e)
        //    {
        //        ImageLoadCount++;
        //        if (imageLoadCount == imageCount)
        //        {
        //            downloading = false;
        //            ImagesLoaded = true;
        //           // Refresh();
        //        }
        //    }, false);

        //    return temp;
        //}

        public void LoadImages()
        {
            if (!ImagesLoaded && !downloading)
            {
                ImageLoadCount = 0;
                ImagesLoaded = false;
                downloading = true;
                bmpBackground = (ImageElement)Document.CreateElement("img");
                bmpBackground.Src = "images/thumbBackground.png";
                bmpBackground.AddEventListener("load", delegate(ElementEvent e)
                {
                    ImageLoadCount++;
                    if (ImageLoadCount == 5)
                    {
                        downloading = false;
                        ImagesLoaded = true;
                        Refresh();
                    }
                }, false);

                bmpBackgroundHover = (ImageElement)Document.CreateElement("img");
                bmpBackgroundHover.Src = "images/thumbBackgroundHover.png";
                bmpBackgroundHover.AddEventListener("load", delegate(ElementEvent e)
                {
                    ImageLoadCount++;
                    if (ImageLoadCount == 5)
                    {
                        downloading = false;
                        ImagesLoaded = true;
                        Refresh();
                    }
                }, false);
                bmpBackgroundWide = (ImageElement)Document.CreateElement("img");
                bmpBackgroundWide.Src = "images/thumbBackgroundWide.png";
                bmpBackgroundWide.AddEventListener("load", delegate(ElementEvent e)
                {
                    ImageLoadCount++;
                    if (ImageLoadCount == 5)
                    {
                        downloading = false;
                        ImagesLoaded = true;
                        Refresh();
                    }
                }, false);
                bmpBackgroundWideHover = (ImageElement)Document.CreateElement("img");
                bmpBackgroundWideHover.Src = "images/thumbBackgroundWideHover.png";
                bmpBackgroundWideHover.AddEventListener("load", delegate(ElementEvent e)
                {
                    ImageLoadCount++;
                    if (ImageLoadCount == 5)
                    {
                        downloading = false;
                        ImagesLoaded = true;
                        Refresh();
                    }
                }, false);
                bmpDropInsertMarker = (ImageElement)Document.CreateElement("img");
                bmpDropInsertMarker.Src = "images/dragInsertMarker.png";
                bmpDropInsertMarker.AddEventListener("load", delegate(ElementEvent e)
                {
                    ImageLoadCount++;
                    if (ImageLoadCount == 5)
                    {
                        downloading = false;
                        ImagesLoaded = true;
                        Refresh();
                    }
                }, false);
            }
        }
        static bool downloading = false;
        static bool ImagesLoaded = false;
        static int ImageLoadCount = 0;
        static ImageElement bmpBackground;
        static ImageElement bmpBackgroundHover;
        static ImageElement bmpBackgroundWide;
        static ImageElement bmpBackgroundWideHover;
        static ImageElement bmpDropInsertMarker;

        ThumbnailSize thumbnailSize = ThumbnailSize.Small;

        public ThumbnailSize ThumbnailSize
        {
            get { return thumbnailSize; }
            set
            {
                thumbnailSize = value;
                switch (value)
                {
                    case ThumbnailSize.Big:
                        HorzSpacing = 180;
                        VertSpacing = 75;
                        ThumbHeight = 65;
                        ThumbWidth = 180;
                        break;
                    case ThumbnailSize.Small:
                        HorzSpacing = 110;
                        VertSpacing = 75;
                        ThumbHeight = 65;
                        ThumbWidth = 110;
                        break;
                }
                UpdatePaginator();
                Refresh();
            }
        }

        public void Refresh()
        {
            if (Width != Window.InnerWidth)
            {
                Width = Window.InnerWidth;
                Canvas.Width = Canvas.Width;
            }
            Paint();
        }

        int HorzSpacing = 110;
        int VertSpacing = 75;
        int ThumbHeight = 65;
        int ThumbWidth = 110;
        float horzMultiple = 110;
        int rowCount = 1;

        public int RowCount
        {
            get { return rowCount; }
            set
            {
                if (rowCount != value)
                {
                    rowCount = value;
                    UpdatePaginator();
                }
            }
        }

        private void UpdatePaginator()
        {
            
        }
        int colCount = 6;

        public int ColCount
        {
            get { return colCount; }
            set
            {
                if (colCount != value)
                {
                    colCount = value;
                    UpdatePaginator();
                }
            }
        }
        bool dragging = false;
        int startIndex = 0;
        int startOffset = 0;
        int selectedItem = -1;
        int hoverItem = -1;

        public int ItemsPerPage
        {
            get
            {
                return rowCount * colCount;
            }
        }



        public int CurrentPage
        {
            get
            {
                return startIndex / ItemsPerPage;
            }
        }

        public bool showAddButton = false;

        public int PageCount
        {
            get
            {
                return Math.Max(1, ((items.Count + ItemsPerPage - 1) + (showAddButton ? 1 : 0)) / ItemsPerPage);
            }
        }
        public int Width;
        public int Height;

        const int buffer = 10;

        public void Paint()
        {

            CanvasContext2D g = (CanvasContext2D)Canvas.GetContext(Rendering.Render2D);
            g.FillStyle = "rgb(20, 22, 31)";
            g.FillRect(0, 0, Width, Height);
            if (!ImagesLoaded)
            {
                return;
            }
            int netHeight = (Height - buffer * 2);
            int netWidth = (Width - buffer * 2);
            RowCount = Math.Round(Math.Max(netHeight / ThumbHeight, 1));
            ColCount = Math.Round(Math.Max(netWidth / HorzSpacing, 1));

            horzMultiple = ((float)netWidth + 13) / (float)ColCount;

            startIndex = Math.Round((startIndex / ItemsPerPage) * ItemsPerPage);

            Rectangle rectf;
            int index = startIndex;
            for (int y = 0; y < rowCount; y++)
            {
                for (int x = 0; x < colCount; x++)
                {
                    if (index >= items.Count)
                    {

                        if (items.Count == 0 || showAddButton)
                        {
                            rectf = Rectangle.Create(Left + x * horzMultiple + 3f + startOffset, Top + y * VertSpacing, ThumbWidth - 10, 60);
                            g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWide : bmpBackground, (int)((float)x * horzMultiple) + startOffset, y * VertSpacing);


                            //g.FillText(emptyText, rectf.X,rectf,Y, rectf.Width);
                            //g.DrawString(showAddButton ? addText : emptyText, UiTools.StandardRegular, (addButtonHover && showAddButton) ? UiTools.YellowTextBrush : UiTools.StadardTextBrush, rectf, UiTools.StringFormatCenterCenter);

                        }
                        break;
                    }




                    rectf = Rectangle.Create(Left + x * horzMultiple + 3 + startOffset, Top + y * VertSpacing, ThumbWidth - 14, 60);
                    //Brush textBrush = UiTools.StadardTextBrush;
                    string textBrush = "white";
                    
                    if (index == hoverItem || (index == selectedItem && hoverItem == -1))
                    {
                        g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWideHover : bmpBackgroundHover, Left + (int)((float)x * horzMultiple) + startOffset, Top + y * VertSpacing);
                        textBrush = "yellow";
                    }
                    else
                    {
                        g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWide : bmpBackground, Left + (int)((float)x * horzMultiple) + startOffset, Top + y * VertSpacing);
                    }

                    (items[index]).Bounds = Rectangle.Create((int)(Left + x * horzMultiple) + startOffset, Top + (int)(y * VertSpacing), (int)horzMultiple, (int)VertSpacing);
                    try
                    {
                        ImageElement bmpThumb = items[index].Thumbnail;
                        if (bmpThumb != null)
                        {

                            g.DrawImage(bmpThumb, Left + (int)(x * horzMultiple) + 2 + startOffset, Top + y * VertSpacing + 3);

                            g.StrokeStyle = "rgb(0,0,0)";
                            g.Rect(Left + (int)((float)x * horzMultiple) + 2 + startOffset, Top + y * VertSpacing + 3, items[index].Thumbnail.Width, items[index].Thumbnail.Height);
                        }
                        else
                        {
                            items[index].Thumbnail = (ImageElement)Document.CreateElement("img");
                            items[index].Thumbnail.Src = items[index].ThumbnailUrl;
                            items[index].Thumbnail.AddEventListener("load", delegate(ElementEvent e) { Refresh(); }, false);
                        }

                    }
                    // TODO FIX this! 
                    catch
                    {
                    }

                    //if (((IThumbnail)items[index]).IsImage)
                    //{
                    //    g.DrawImage(Properties.Resources.InsertPictureHS, (int)((float)x * horzMultiple) + 79, y * VertSpacing + 1);
                    //}
                    //if (((IThumbnail)items[index]).IsTour)
                    //{
                    //    g.DrawImage(Properties.Resources.TourIcon, (int)((float)x * horzMultiple) + 79, y * VertSpacing + 1);
                    //}
                    //g.DrawString(((IThumbnail), UiTools.StandardRegular, textBrush, rectf, UiTools.StringFormatThumbnails);
                    g.FillStyle = textBrush;
                    g.StrokeStyle = textBrush;
                    g.LineWidth = 1;
                    g.Font = "normal 8pt Arial";
                    g.FillText(items[index].Name, rectf.X, rectf.Y + rectf.Height, rectf.Width);
                    //g.FillText(items[index].Name, 10, 10);
                    index++;
                }
                if (index >= items.Count)
                {
                    break;
                }
            }
        }

        bool addButtonHover = false;
        public bool imageClicked = false;
        private int GetItemIndexFromCursor(Vector2d testPointIn)
        {
            Vector2d testPoint = Vector2d.Create(testPointIn.X + Left, testPointIn.Y + Top);

            imageClicked = false;
            int index = -1;
            int xpos = (int)((float)testPoint.X / horzMultiple);
            int xPart = (int)((float)testPoint.X % horzMultiple);
            if (xpos >= colCount)
            {
                return -1;
            }
            if (xpos < 0)
            {
                return -1;
            }

            int ypos = (int)(testPoint.Y / VertSpacing);
            int yPart = (int)(testPoint.Y % VertSpacing);
            if (ypos >= rowCount)
            {
                return -1;
            }
            
            if (ypos < 0)
            {
                return -1;
            }

            index = startIndex + ypos * colCount + xpos;

            if (index == items.Count)
            {
                addButtonHover = true;
            }
            else
            {
                addButtonHover = false;
            }

            if (index > items.Count-1)
            {
                return -1;
            }

            if (((IThumbnail)items[index]).IsImage && yPart < 16 && xPart > 78)
            {
                imageClicked = true;
            }

            return index;
        }

        internal void AddItems(List<IThumbnail> list)
        {
            items = list;
        }


    }
    public enum ThumbnailSize { Small=0, Big=1 };

}
