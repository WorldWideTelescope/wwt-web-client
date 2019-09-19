using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;

namespace wwtlib
{
    public class Table
    {
        public Table()
        {
        }

        public Guid Guid = new Guid();
        public List<string> Header = new List<string>();
        public List<List<string>> Rows = new List<List<string>>();
        public string Delimiter = "\t";
        public bool Locked = false;

      //  private Mutex tableMutex = new Mutex();

        public void Lock()
        {
            Locked = true;
    //        tableMutex.WaitOne();
        }

        public void Unlock()
        {
            Locked = false;
    //        tableMutex.ReleaseMutex();
        }

        public string Save()
        {
            string data = "";

            bool first = true;

            foreach (string col in Header)
            {
                if (!first)
                {
                    data += "\t";
                }
                else
                {
                    first = false;
                }

                data += col;
            }
            data += "\r\n";
            foreach (string[] row in Rows)
            {
                first = true;
                foreach (string col in row)
                {
                    if (!first)
                    {
                        data += "\t";
                    }
                    else
                    {
                        first = false;
                    }

                    data += col;
                }
                data += "\r\n";
            }

            return data;
        }

        //public void Save(string path)
        //{

        //    using (Stream s = new FileStream(path, FileMode.Create))
        //    {
        //        Save(s);
        //    }
        //}
        //public void Save(Stream stream)
        //{
        //    stream = new GZipStream(stream, CompressionMode.Compress);

        //    StreamWriter sw = new StreamWriter(stream, Encoding.UTF8);
        //    bool first = true;

        //    foreach (string col in Header)
        //    {
        //        if (!first)
        //        {
        //            sw.Write("\t");
        //        }
        //        else
        //        {
        //            first = false;
        //        }

        //        sw.Write(col);
        //    }
        //    sw.Write("\r\n");
        //    foreach (string[] row in Rows)
        //    {
        //        first = true;
        //        foreach (string col in row)
        //        {
        //            if (!first)
        //            {
        //                sw.Write("\t");
        //            }
        //            else
        //            {
        //                first = false;
        //            }

        //            sw.Write(col);
        //        }
        //        sw.Write("\r\n");
        //    }
        //    sw.Close();


        //}

        //public override string ToString()
        //{
        //    StringBuilder sb = new StringBuilder();

        //    StringWriter sw = new StringWriter(sb);
        //    bool first = true;

        //    foreach (string col in Header)
        //    {
        //        if (!first)
        //        {
        //            sw.Write("\t");
        //        }
        //        else
        //        {
        //            first = false;
        //        }

        //        sw.Write(col);
        //    }
        //    sw.Write("\r\n");
        //    foreach (string[] row in Rows)
        //    {
        //        first = true;
        //        foreach (string col in row)
        //        {
        //            if (!first)
        //            {
        //                sw.Write("\t");
        //            }
        //            else
        //            {
        //                first = false;
        //            }

        //            sw.Write(col);
        //        }
        //        sw.Write("\r\n");
        //    }
        //    return sb.ToString();

        //}

        //public static bool IsGzip(Stream stream)
        //{
        //    BinaryReader br = new BinaryReader(stream);
        //    byte[] line = br.ReadBytes(2);

        //    if (line[0] == 31 && line[1] == 139)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static Table Load(string path, char delimiter)
        //{
        //    if (path.ToLower().EndsWith("csv"))
        //    {
        //        delimiter = ',';
        //    }

        //    using (Stream s = new FileStream(path, FileMode.Open))
        //    {
        //        return Load(s, delimiter);
        //    }
        //}

        //public static Table Load(Stream stream, char delimiter)
        //{

        //    bool gZip = IsGzip(stream);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    if (gZip)
        //    {
        //        stream = new GZipStream(stream, CompressionMode.Decompress);
        //    }

        //    Table table = new Table();
        //    table.Delimiter = delimiter;

        //    StreamReader sr = new StreamReader(stream);

        //    if (sr.Peek() >= 0)
        //    {
        //        string headerLine = sr.ReadLine();
        //        table.Rows.Clear();
        //        table.Header = UiTools.SplitString(headerLine, delimiter);
        //    }
        //    else
        //    {
        //        table.Header = new string[0];
        //    }

        //    int count = 0;
        //    while (sr.Peek() >= 0)
        //    {
        //        string line = sr.ReadLine();
        //        string[] rowData = UiTools.SplitString(line, delimiter);
        //        if (rowData.Length < 2)
        //        {
        //            break;
        //        }
        //        table.Rows.Add(rowData);
        //        count++;
        //    }
        //    return table;
        //}

        public void LoadFromString(string data, bool isUpdate, bool purge, bool hasHeader)
        {
            int count = 0;
            string[] lines = data.Split("\r\n");
            if (!isUpdate || hasHeader)
            {
                if (lines.Length > 0)
                {
                    string headerLine = lines[0];
                    count++;
                    if (headerLine.IndexOf("\t") == -1 && headerLine.IndexOf(",") > -1)
                    {
                        Delimiter = ",";
                    }

                    if (!isUpdate)
                    {
                        Rows.Clear();
                    }
                    Header = UiTools.SplitString(headerLine, Delimiter);
                }
                else
                {
                    Header = new List<string>();
                }
            }
            List<List<string>> temp = new List<List<string>>();
            if (!purge)
            {
                temp = Rows;
            }


            while (count  < lines.Length)
            {
                string line = lines[count];
                List<string> rowData = UiTools.SplitString(line, Delimiter);
                if (rowData.Count < 1)
                {
                    break;
                }
                temp.Add(rowData);
                count++;
            }
            if (purge)
            {
                Rows = temp;
            }

        }

        //public void Append(string data)
        //{
        //    StringReader sr = new StringReader(data);
        //    int count = 0;
        //    while (sr.Peek() >= 0)
        //    {
        //        string line = sr.ReadLine();
        //        string[] rowData = UiTools.SplitString(line, Delimiter);
        //        if (rowData.Length < 2)
        //        {
        //            break;
        //        }
        //        Rows.Add(rowData);
        //        count++;
        //    }
        //}

        public Table Clone()
        {
            Table cloned_table = new Table();
            for (int i = 0; i < Header.Count; i++)
            {
                cloned_table.Header.Add(Header[i]);
            }
            for (int j = 0; j < Rows.Count; j++)
            {
                cloned_table.Rows.Add(new List<string>());
                for (int i = 0; i < Rows[j].Count; i++)
                {
                    cloned_table.Rows[j].Add(Rows[j][i]);
                }
            }
            return cloned_table;
        }

        public void AddColumn(string name, List<string> data)
        {
            Header.Add(name);
            for (int i = 0; i < data.Count; i++)
            {
                Rows[i].Add(data[i]);
            }
        }

        public void RemoveColumn(string name) {
            int remove_index = Header.IndexOf(name);
            if (remove_index > -1) {
                Header.RemoveAt(remove_index);
                for (int i = 0; i < Rows.Count; i++)
                    Rows[i].RemoveAt(remove_index);
                }
            }
        }
    }
}
