using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;


namespace wwtlib
{
    public class ColorPicker
    {
        public ColorPicker()
        {

        }

        public void NonMenuClick(ElementEvent e)
        {
            //DivElement menu = Document.GetElementById<DivElement>("colorpicker");
            //menu.Style.Display = "none";
            //Window.RemoveEventListener("click", NonMenuClick, true);

            //ImageElement image = Document.GetElementById<ImageElement>("colorhex");
            //image.RemoveEventListener("click", PickColor, false);
        }


        public void Show(EventArgs e)
        {
            WWTControl.scriptInterface.ShowColorPicker(this, e);
            //DivElement picker = Document.GetElementById<DivElement>("colorpicker");
            //picker.ClassName = "colorpicker";
            //picker.Style.Display = "block";
            //picker.Style.Left = position.X.ToString() + "px";
            //picker.Style.Top = position.Y.ToString() + "px";

            //Window.AddEventListener("click", NonMenuClick, true);

            //ImageElement image = Document.GetElementById<ImageElement>("colorhex");

            //image.AddEventListener("mousedown", PickColor, false);
        }

        public Color GetColorFromClick(ElementEvent e)
        {
            ImageElement image = Document.GetElementById<ImageElement>("colorhex");

            CanvasElement canvas = (CanvasElement)Document.CreateElement("canvas");
            canvas.Width = image.Width;
            canvas.Height = image.Height;

            CanvasContext2D ctx = (CanvasContext2D)canvas.GetContext(Rendering.Render2D);
            ctx.DrawImage(image, 0, 0);

            PixelArray pixels = ctx.GetImageData(e.OffsetX, e.OffsetY, 1, 1).Data;
            Color = Color.FromArgb((float)pixels[3], (float)pixels[0], (float)pixels[1], (float)pixels[2]);

            return Color;

        }

        public void PickColor(ElementEvent e)
        {
            
            CallBack(Color);

        }

        public ColorPick CallBack = null;

        public Color Color = Colors.White;
    }

   
    public delegate void ColorPick(Color Picked);

}
