using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{

    public class XmlTextWriter
    {

        public string Body = "<?xml version='1.0' encoding='UTF-8'?>\r\n";
        public Formatting Formatting = Formatting.Indented;
        Stack<string> elementStack = new Stack<string>();
        bool pending = false;
        string currentName = "";
        Dictionary<string, string> attributes = new Dictionary<string, string>();
        string value = "";
        void PushNewElement( string name)
        {
            //write pending element and attributes

            WritePending(false);

            //Push new attribute on to stack
            elementStack.Push(name);

            //setup pending structures
            pending = true;
            currentName = name;
            

        }

        private bool WritePending(bool fullClose)
        {
            bool closed = true;
            if (pending)
            {
                for (int i = 1; i < elementStack.Count; i++)
                {
                    Body += "  ";
                }

                Body += "<" + currentName;
                if (attributes.Count > 0)
                {
                    foreach (string key in attributes.Keys)
                    {
                        Body += string.Format(" {0}=\"{1}\"", key, attributes[key]);
                    }
                }
                               
                if (!string.IsNullOrEmpty(value))
                {               
                    Body += ">";
                    closed = false;
                    if (!string.IsNullOrEmpty(value))
                    {
                        Body += value;
                    }
                }
                else
                {
                    if (fullClose)
                    {
                        Body += " />\r\n";
                        closed = true;
                    }
                    else
                    {
                        Body += ">\r\n";
                    }
                }

                pending = false;
                currentName = "";
                value = "";
                attributes = new Dictionary<string, string>();
                return closed;
            }

            return false;
        }

        internal void WriteProcessingInstruction(string v1, string v2)
        {
            
        }

        internal void WriteStartElement(string name)
        {
            PushNewElement(name);
        }

        internal void WriteAttributeString(string key, object value)
        {
            if (value != null)
            {
                attributes[key] = value.ToString().Replace("&", "&amp;");
            }
            else
            {
                attributes[key] = "";
            }
        }

        internal void WriteEndElement()
        {
            if (!WritePending(true))
            {
                for (int i = 1; i < elementStack.Count; i++)
                {
                    Body += "  ";
                }
                Body += string.Format("</{0}>\r\n", elementStack.Pop());
            }
            else
            {
                elementStack.Pop();
            }
        }

        internal void WriteString(string text)
        {
            value = text.Replace("&", "&amp;");
        }

        internal void WriteFullEndElement()
        {
            WritePending(false);
            for (int i = 1; i < elementStack.Count; i++)
            {
                Body += "  ";
            }
            Body += string.Format("</{0}>\r\n", elementStack.Pop());     
        }

        internal void Close()
        {
        }

        internal void WriteElementString(string name, string value)
        {
            WriteStartElement(name);
            WriteValue(value.Replace("&", "&amp;"));
            WriteEndElement();
        }

        internal void WriteValue(string val)
        {
            value = val.Replace("&", "&amp;");
        }


        internal void WriteCData(string htmlDescription)
        {
            value = string.Format("<![CDATA[{0}]]>", htmlDescription);
        }
    }

    public enum Formatting { Indented = 1 };
}
