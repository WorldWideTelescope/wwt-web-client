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
            temp.ForegroundColor = forgroundColor;
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
        public Color ForegroundColor;
        public Color BackgroundColor;
        public TextBorderStyle BorderStyle;

        public override string ToString()
        {
            return Text;
        }

        internal void SaveToXml(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("TextObject");
            xmlWriter.WriteAttributeString("Bold", Bold.ToString());
            xmlWriter.WriteAttributeString("Italic", Italic.ToString());
            xmlWriter.WriteAttributeString("Underline", Underline.ToString());
            xmlWriter.WriteAttributeString("FontSize", FontSize.ToString());
            xmlWriter.WriteAttributeString("FontName", FontName);
            xmlWriter.WriteAttributeString("ForgroundColor", ForegroundColor.Save());
            xmlWriter.WriteAttributeString("BackgroundColor", BackgroundColor.Save());
            xmlWriter.WriteAttributeString("BorderStyle", Enums.ToXml("TextBorderStyle", (int)BorderStyle));

            xmlWriter.WriteString(this.Text);
            xmlWriter.WriteEndElement();
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
            newTextObject.ForegroundColor = Color.Load(node.Attributes.GetNamedItem("ForgroundColor").Value);
            newTextObject.BackgroundColor = Color.Load(node.Attributes.GetNamedItem("BackgroundColor").Value);
            if (node.Attributes.GetNamedItem("BorderStyle") != null)
            {
                newTextObject.BorderStyle = (TextBorderStyle)Enums.Parse("TextBorderStyle",node.Attributes.GetNamedItem("BorderStyle").Value);
            }
            return newTextObject;
        }
    }
}
