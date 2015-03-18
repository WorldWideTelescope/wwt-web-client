<%@ Page Language="C#" ContentType="application/octet-stream" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="Microsoft.Maps.ElevationAdjustmentService.HDPhoto" %>
<%
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);
        int demSize = 33 * 33;
	string wwtDemDir = ConfigurationManager.AppSettings["WWTDEMDir"];
        string filename = String.Format(wwtDemDir  + @"\Mercator\Chunks\{0}\{1}.chunk", level, tileY);
	string urlBase = "http://ecn.t{0}.tiles.virtualearth.net/tiles/d{1}.elv?g=1&n=z";
	
	string id = TileTools.GetTileID(level,tileX,tileY);
	int server = TileTools.GetServerID(tileX,tileY);
	WebClient client = new WebClient();
	
	string url = string.Format(urlBase,server,id);
	
	byte[] data = client.DownloadData(url);
	MemoryStream stream = new MemoryStream(data);


	DemTile tile = DemCodec.Decompress(stream);

	 if( tile != null)
         {
                float[] DemData = new float[demSize];
                int yh = 0;
                for (int yl = 0; yl < 33;  yl++)
                {
                    int xh = 0;
                    for (int xl = 0; xl < 33; xl++)
                    {
                        int indexI = xl + (32-yl) * 33;
                        DemData[indexI] = (float)tile.AltitudeInMeters(yh, xh);
                      //  Response.Write( tile.AltitudeInMeters(yh, xh).ToString() + "\n");	  

                        xh += 8;
//Response.Write(indexI);
                    }
                    yh += 8;

                 }
		data = new byte[DemData.Length*4];
		MemoryStream ms = new MemoryStream(data);
		BinaryWriter bw = new BinaryWriter(ms);
		
		foreach(float sample in DemData)
		{
		    bw.Write(sample);
		}
		bw.Flush();
		Response.BinaryWrite(data);
	}

        Response.End();
	
	%>