using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;

namespace wwtlib
{

    public enum TextBorderStyle { None=0, Tight=1, Small=2, Medium=3, Large=4 };
    public class TextObject
    {
        public TextObject()
        {
        }


        public static TextObject Create(string text, bool bold, bool italic, bool underline, float fontSize, string fontName, Color forgroundColor, Color backgroundColor , TextBorderStyle borderStyle)
        {
            TextObject temp = new TextObject();

            temp.Text = text;
            temp.Bold = bold;
            temp.Italic = italic;
            temp.Underline = underline;
            temp.FontSize = fontSize;
            temp.FontName = fontName;
            temp.ForgroundColor = forgroundColor;
            temp.BackgroundColor = backgroundColor;
            temp.BorderStyle = borderStyle;
            return temp;
        }

        public string Text;
        public bool Bold;
        public bool Italic;
        public bool Underline;
        public float FontSize;
        public string FontName;
        public Color ForgroundColor;
        public Color BackgroundColor;
        public TextBorderStyle BorderStyle;


        public override string ToString()
        {
            return Text;
        }

        internal static TextObject FromXml(XmlNode node)
        {
            TextObject newTextObject = new TextObject();
            newTextObject.Text = Util.GetInnerText(node);
            newTextObject.BorderStyle = TextBorderStyle.None;
            newTextObject.Bold = bool.Parse(node.Attributes.GetNamedItem("Bold").Value);
            newTextObject.Italic = bool.Parse(node.Attributes.GetNamedItem("Italic").Value);
            newTextObject.Underline = bool.Parse(node.Attributes.GetNamedItem("Underline").Value);
            newTextObject.FontSize = float.Parse(node.Attributes.GetNamedItem("FontSize").Value);
            newTextObject.FontName = node.Attributes.GetNamedItem("FontName").Value;
            newTextObject.ForgroundColor = Color.Load(node.Attributes.GetNamedItem("ForgroundColor").Value);
            newTextObject.BackgroundColor = Color.Load(node.Attributes.GetNamedItem("BackgroundColor").Value);
            if (node.Attributes.GetNamedItem("BorderStyle") != null)
            {
                switch (node.Attributes.GetNamedItem("BorderStyle").Value)
                {
                   case "None":
                        newTextObject.BorderStyle = TextBorderStyle.None;
                        break;
                    case "Tight":
                        newTextObject.BorderStyle = TextBorderStyle.Tight;
                        break;
                    case "Small":
                        newTextObject.BorderStyle = TextBorderStyle.Small;
                        break;
                    case "Medium":
                        newTextObject.BorderStyle = TextBorderStyle.Medium;
                        break;
                    case "Large":
                        newTextObject.BorderStyle = TextBorderStyle.Large;
                        break;
                    default:
                        break;
                }
            }
            return newTextObject;
        }
    }
}
