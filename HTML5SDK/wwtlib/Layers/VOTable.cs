using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Net;
using System.Html.Data.Files;

namespace wwtlib
{
    public enum Primitives
    {
        VoBoolean = 1,
        VoBit = 2,
        VoUnsignedByte = 3,
        VoShort = 4,
        VoInt = 5,
        VoLong = 6,
        VoChar = 7,
        VoUnicodeChar = 8,
        VoFloat = 9,
        VoDouble = 10,
        VoFloatComplex = 11,
        VoDoubleComplex = 12,
        VoUndefined = 13
    };

    public class VoTable
    {
        public Dictionary<string, VoColumn> Columns = new Dictionary<string, VoColumn>();
        public List<VoColumn> Column = new List<VoColumn>();
        public List<VoRow> Rows = new List<VoRow>();
        public string LoadFilename = "";
        public string Url;
        public string SampId = "";
        public VoRow SelectedRow = null;

        public VoTable()
        {

        }
        //public VoTable(XmlDocument xml)
        //{
        //    LoadFromXML(xml);
        //}

        //public VoTable(string filename)
        //{
        //    LoadFilename = filename;
        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(filename);
        //    LoadFromXML(doc);
        //}

        WebFile webFile;
        Action onComplete;
        public static VoTable LoadFromUrl(string url, Action complete)
        {
            VoTable temp = new VoTable();

            temp.onComplete = complete;

            temp.webFile = new WebFile(Util.GetProxiedUrl(url));
            temp.webFile.OnStateChange = temp.LoadData;
            temp.webFile.Send();

            return temp;

        }

        private void LoadData()
        {
            if (webFile.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFile.Message);
            }
            else if (webFile.State == StateType.Received)
            {
                LoadFromXML(webFile.GetXml());

                if (onComplete != null)
                {
                    onComplete();
                }
            }
        }

        public static VoTable LoadFromString(string data)
        {
            XmlDocumentParser xParser = new XmlDocumentParser();
            XmlDocument doc = xParser.ParseFromString(data, "text/xml");

            VoTable table = new VoTable();

            table.LoadFromXML(doc);

            return table;
        }


        public bool error = false;
        public string errorText = "";
        public void LoadFromXML(XmlDocument xml)
        {
            XmlNode voTable = Util.SelectSingleNode(xml, "VOTABLE"); 

            if (voTable == null)
            {
                return;
            }
            int index = 0;
            try
            {
                XmlNode table = Util.SelectSingleNode(Util.SelectSingleNode(voTable, "RESOURCE"),"TABLE");
                if (table != null)
                {
                    foreach (XmlNode node in table.ChildNodes)
                    {
                        if (node.Name == "FIELD")
                        {
                            VoColumn col = new VoColumn(node, index++);
                            Columns[col.Name] = col;
                            Column.Add(col);
                        }
                    }
                }
            }
            catch
            {
                error = true;
                errorText = Util.SelectSingleNode(voTable,"DESCRIPTION").InnerText.ToString();
            }
            try
            {
                XmlNode tableData = Util.SelectSingleNode(Util.SelectSingleNode(Util.SelectSingleNode(Util.SelectSingleNode(voTable,"RESOURCE"),"TABLE"),"DATA"),"TABLEDATA");
                if (tableData != null)
                {
                    foreach (XmlNode node in tableData.ChildNodes)
                    {
                        if (node.Name == "TR")
                        {
                            VoRow row = new VoRow(this);
                            row.ColumnData = new object[Columns.Count];
                            index = 0;
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                if (child.Name == "TD")
                                {
                                    row.ColumnData[index++] = Util.GetInnerText(child).Trim();
                                }
                            }
                            Rows.Add(row);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public bool Save(string filename)
        {
            //todo add save
            //if (String.IsNullOrEmpty(filename) || String.IsNullOrEmpty(LoadFilename))
            //{
            //    return false;
            //}
            //try
            //{
            //    File.Copy(LoadFilename, filename);
            //}
            //catch
            //{
            //    return false;
            //}
            return true;

        }
        public VoColumn GetColumnByUcd(string ucd)
        {
            foreach (string key in Columns.Keys)
            {
                VoColumn col = Columns[key];
                if (col.Ucd.Replace("_", ".").ToLocaleLowerCase().IndexOf(ucd.ToLocaleLowerCase()) > -1)
                {
                    return col;
                }
            }
            return null;
        }

        public VoColumn GetRAColumn()
        {
            foreach (string key in Columns.Keys)
            {
                VoColumn col = Columns[key];
                if (col.Ucd.ToLocaleLowerCase().IndexOf("pos.eq.ra") > -1 || col.Ucd.ToLocaleLowerCase().IndexOf("pos_eq_ra") > -1)
                {
                    return col;
                }
            }

            foreach (string key in Columns.Keys)
            {
                VoColumn col = Columns[key];
                if (col.Name.ToLocaleLowerCase().IndexOf("ra") > -1)
                {
                    return col;
                }
            }

            return null;
        }

        public VoColumn GetDecColumn()
        {
            foreach (string key in Columns.Keys)
            {
                VoColumn col = Columns[key];
                if (col.Ucd.ToLowerCase().IndexOf("pos.eq.dec") > -1 || col.Ucd.ToLowerCase().IndexOf("pos_eq_dec") > -1)
                {
                    return col;
                }
            }

            foreach (string key in Columns.Keys)
            {
                VoColumn col = Columns[key];
                if (col.Name.ToLowerCase().IndexOf("dec") > -1)
                {
                    return col;
                }
            }
            return null;
        }

        public VoColumn GetDistanceColumn()
        {
            foreach (string key in Columns.Keys)
            {
                VoColumn col = Columns[key];
                if (col.Ucd.ToLowerCase().IndexOf("pos.distance")  > -1 || col.Ucd.ToLowerCase().IndexOf("pos_distance") > -1)
                {
                    return col;
                }
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            bool first = true;
            // Copy header
            foreach (string key in Columns.Keys)
            {
                VoColumn col = Columns[key];
                if (first)
                {
                     first = false;
                }
                else
                {
                   sb.Append("\t");
                }

                sb.Append(col.Name);
            }
            sb.AppendLine("");

            // copy rows

            foreach (VoRow row in Rows)
            {
                first = true;
                foreach (object col in row.ColumnData)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append("\t");
                    }

                    sb.Append(col.ToString());
                }
                sb.AppendLine("");
            }
            return sb.ToString();
        }
    }

    public class VoRow
    {
        public bool Selected = false;
        public VoTable Owner;
        public object[] ColumnData;
        public VoRow(VoTable owner)
        {
            Owner = owner;
        }
        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= ColumnData.Length)
                {
                    return null;
                }
                return ColumnData[index];
            }
        }
        //public object this[string key]
        //{
        //    get
        //    {
        //        if (Owner.Columns[key] != null)
        //        {
        //            return ColumnData[Owner.Columns[key].Index];
        //        }
        //        return null;
        //    }
        //}

        public object GetColumnData(string key)
        {
            if (Owner.Columns[key] != null)
            {
                return ColumnData[Owner.Columns[key].Index];
            }
            return null;
        }
    }

    public class VoColumn
    {
        public VoColumn(XmlNode node, int index)
        {
            Index = index;
            if (node.Attributes.GetNamedItem("datatype") != null)
            {
                Type = GetType(node.Attributes.GetNamedItem("datatype").Value);
            }
            if (node.Attributes.GetNamedItem("ucd") != null)
            {
                Ucd = node.Attributes.GetNamedItem("ucd").Value;
            }
            if (node.Attributes.GetNamedItem("precision") != null)
            {
                try
                {
                    Precision = Int32.Parse(node.Attributes.GetNamedItem("precision").Value);
                }
                catch
                {
                }
            }
            if (node.Attributes.GetNamedItem("ID") != null)
            {
                Id = node.Attributes.GetNamedItem("ID").Value;
            }       
            
            if (node.Attributes.GetNamedItem("name") != null)
            {
                Name = node.Attributes.GetNamedItem("name").Value;
            }
            else
            {
                Name = Id;
            }

            if (node.Attributes.GetNamedItem("unit") != null)
            {
                Unit = node.Attributes.GetNamedItem("unit").Value;
            }

            
            if (node.Attributes.GetNamedItem("arraysize") != null)
            {
                string[] split = node.Attributes.GetNamedItem("arraysize").Value.Split( 'x' );
                Dimentions = split.Length;
                Sizes = new int[split.Length];
                int indexer = 0;
                foreach (string dim in split)
                {
                    if (!(dim.IndexOf("*") > -1))
                    {
                        Sizes[indexer++] = Int32.Parse(dim);
                    }
                    else
                    {
                        int len = 9999;
                        string lenString = dim.Replace("*","");
                        if (lenString.Length > 0)
                        {
                            len = Int32.Parse(lenString);
                        }
                        Sizes[indexer++] = len;
                        
                    }
                }
            }

        }
        public string Id = "";
        public Primitives Type;
        public int Precision = 0;
        public int Dimentions = 0;
        public int[] Sizes = null;
        public string Ucd = "";
        public string Unit = "";
        public string Name = "";
        public int Index;

        public static Primitives GetType(string type)
        {
            Primitives Type = Primitives.VoUndefined;
            switch (type)
            {
                case "boolean":
                    Type = Primitives.VoBoolean;
                    break;
                case "bit":
                    Type = Primitives.VoBit;
                    break;
                case "unsignedByte":
                    Type = Primitives.VoUnsignedByte;
                    break;
                case "short":
                    Type = Primitives.VoShort;
                    break;
                case "int":
                    Type = Primitives.VoInt;
                    break;
                case "long":
                    Type = Primitives.VoLong;
                    break;
                case "char":
                    Type = Primitives.VoChar;
                    break;
                case "unicodeChar":
                    Type = Primitives.VoUnicodeChar;
                    break;
                case "float":
                    Type = Primitives.VoFloat;
                    break;
                case "double":
                    Type = Primitives.VoDouble;
                    break;
                case "floatComplex":
                    Type = Primitives.VoFloatComplex;
                    break;
                case "doubleComplex":
                    Type = Primitives.VoDoubleComplex;
                    break;
                default:
                    Type = Primitives.VoUndefined;
                    break;

            }
            return Type;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
