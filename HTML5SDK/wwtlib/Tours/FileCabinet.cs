using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Data.Files;

namespace wwtlib
{

    public class FileEntry
    {
        public string Filename;
        public int Size;
        public int Offset;
        public Blob Blob;
        public FileEntry(string filename, int size)
        {
            Filename = filename;
            Size = size;
        }
        public override string ToString()
        {
            return Filename;
        }
    }

    public class FileCabinet
    {
        protected List<FileEntry> FileList;
        Dictionary<string, FileEntry> FileDirectory;
        public string Filename;
        public string TempDirectory = "";
        private int currentOffset = 0;
        private string packageID = "";

        public string PackageID
        {
            get { return packageID; }
            set { packageID = value; }
        }

        //public FileCabinet(string filename, string directory)
        //{
        //    ClearFileList();
        //    Filename = filename;
        //    TempDirectory = directory;
        //}

        public FileCabinet()
        {
            ClearFileList();
        }

        public void AddFile(string filename, Blob data)
        {
            if (data == null)
            {
                return;
            }
          
            if (!FileDirectory.ContainsKey(filename))
            {
                FileEntry fe = new FileEntry(filename, (int)data.Size);
                fe.Offset = currentOffset;
                fe.Blob = data;
                FileList.Add(fe);
                FileDirectory[filename] = fe;
                currentOffset += fe.Size;
            }
        }

        public void ClearFileList()
        {
            if (FileList == null)
            {
                FileList = new List<FileEntry>();
            }
            if (FileDirectory == null)
            {
                FileDirectory = new Dictionary<string, FileEntry>();
            }
            FileList.Clear();
            FileDirectory.Clear();
            currentOffset = 0;
        }

        public Blob PackageFiles()
        {
            XmlTextWriter xmlWriter = new XmlTextWriter();
           
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xmlWriter.WriteStartElement("FileCabinet");
            xmlWriter.WriteAttributeString("HeaderSize", "0x0BADFOOD");

            xmlWriter.WriteStartElement("Files");
            foreach (FileEntry entry in FileList)
            {
                xmlWriter.WriteStartElement("File");
                xmlWriter.WriteAttributeString("Name", entry.Filename);
                xmlWriter.WriteAttributeString("Size", entry.Size.ToString());
                xmlWriter.WriteAttributeString("Offset", entry.Offset.ToString());
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteFullEndElement();
            xmlWriter.Close();

            string data = xmlWriter.Body;

            Blob blob = new Blob(new object[] { data });

            string sizeText = String.Format("0x{0:x8}", blob.Size);

            data = data.Replace("0x0BADFOOD", sizeText);

            blob = new Blob(new object[] { data });

            List<Blob> blobs = new List<Blob>();

            blobs.Add(blob);

            // add the blobs to array to append in order
            foreach (FileEntry entry in FileList)
            {   
                blobs.Add(entry.Blob);
            }

            Blob cabBlob = (Blob)Script.Literal("new Blob({0}, {{type : 'application/x-wtt'}});", blobs);

            return cabBlob;
        }


        public string Url = "";
        private WebFile webFile;
        private Action callMe;
        private System.Html.Data.Files.Blob mainBlob;

        public static FileCabinet FromUrl(string url, Action callMe)
        {

            FileCabinet temp = new FileCabinet();
            temp.Url = url;
            temp.callMe = callMe;

            temp.webFile = new WebFile(url);
            temp.webFile.ResponseType = "blob";
            temp.webFile.OnStateChange = temp.LoadCabinet;
            temp.webFile.Send();

            return temp;
        }

        private void LoadCabinet()
        {
            if (webFile.State == StateType.Error)
            {
                Script.Literal("alert({0})", webFile.Message);
            }
            else if (webFile.State == StateType.Received)
            {
                mainBlob = (System.Html.Data.Files.Blob)webFile.GetBlob();
                FileReader chunck = new FileReader();
                chunck.OnLoadEnd = delegate (System.Html.Data.Files.FileProgressEvent e)
                {
                    int offset = GetSize((string)chunck.Result);

                    FileReader header = new FileReader();
                    header.OnLoadEnd = delegate (System.Html.Data.Files.FileProgressEvent ee)
                    {
                        string data = header.Result as string;
                        XmlDocumentParser xParser = new XmlDocumentParser();
                        Extract(xParser.ParseFromString(data, "text/xml"), offset);
                        callMe();
                    };
                    header.ReadAsText(mainBlob.Slice(0, offset));
                };
                chunck.ReadAsText(mainBlob.Slice(0, 255));
                
            }
        }

        private int GetSize(string data)
        {
            int start = data.IndexOf("0x");
            if (start == -1)
            {
                return 0;
            }
            return int.Parse(data.Substring(start, start + 10), 16);
        }

        public void Extract(XmlDocument doc, int offset)
        {
            try
            {
                XmlNode cab = Util.SelectSingleNode(doc, "FileCabinet");
                XmlNode files = Util.SelectSingleNode(cab, "Files");

                FileList.Clear();
                foreach (XmlNode child in files.ChildNodes)
                {
                    if (child.Name == "File")
                    {
                        FileEntry fe = new FileEntry(child.Attributes.GetNamedItem("Name").Value, int.Parse(child.Attributes.GetNamedItem("Size").Value));
                        fe.Offset = offset;
                        offset += fe.Size;
                        FileList.Add(fe);
                    }
                }
            }
            catch
            {
                //  UiTools.ShowMessageBox("The data cabinet file was not found. WWT will now download all data from network.");
            }
        }

        public Blob GetFileBlob(string filename)
        {
            FileEntry fe = GetFileEntry(filename);
            if (fe != null)
            {
                string ext = filename.Substr(filename.LastIndexOf(".")).ToLowerCase();
                string type = null;

                switch (ext)
                {
                    case ".png":
                        type = "image/png";
                        break;
                    case ".jpg":
                    case ".jpeg":
                        type = "image/jpeg";
                        break;
                    case ".mp3":
                        type = "audio/mpeg3";
                        break;
                    case ".txt":
                        type = "text/plain";
                        break;
                    case ".fit":
                    case ".fits":
                        type = "application/octet-stream";
                        break;
                }


                return mainBlob.Slice(fe.Offset, fe.Offset + fe.Size, type);
            }

            return null;
        }

        public FileEntry GetFileEntry(string filename)
        {
            foreach (FileEntry entry in FileList)
            {
                if (entry.Filename == filename)
                {
                    return entry;
                }
            }

            return null;
        }

        public string MasterFile
        {
            get
            {
                if (FileList.Count > 0)
                {
                    return FileList[0].Filename;
                }
                else
                {
                    return null;
                }
            }
        }
        public void ClearTempFiles()
        {
            foreach (FileEntry entry in FileList)
            {
               //tofo release file URL's from blobs
            }
        }

    }
}