<%@ Page Language="C#" ContentType="image/jpeg" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Text" %>
<%@ Import Namespace="System.Drawing.Imaging" %>
<%@ Import Namespace="System.Drawing.Drawing2D" %>
<%@ Import Namespace="System.IO" %>
<%
        string query = Request.Params["Q"];
        string[] values = query.Split(',');   
        int level = Convert.ToInt32(values[0]);
        int tileX = Convert.ToInt32(values[1]);
        int tileY = Convert.ToInt32(values[2]);

        string filename;
        string path;
        Bitmap b = null;
    	string DSSTileCache = Util.GetCurrentConfigShare("DSSTileCache", true);
	string webDir = ConfigurationManager.AppSettings["WWTWEBDIR"];


        filename = String.Format( DSSTileCache  + "\\wwtcache\\mandel\\level{0}\\{2}\\{1}_{2}.png", level, tileX, tileY);
        path = String.Format(DSSTileCache + "\\wwtcache\\mandel\\level{0}\\{2}", level, tileX, tileY);

        if ((level < 32) && File.Exists(filename))
        {
            try
            {
                Response.WriteFile(filename);
            }
            catch
            {
                b = null;
            }
        }
        else
        {
            double tileWidth = (4 / (Math.Pow(2, level)));
            double Sy = ((double)tileY * tileWidth) - 2;
            double Fy = Sy + tileWidth;
            double Sx = ((double)tileX * tileWidth) - 4;
            double Fx = Sx + tileWidth;


            Response.Clear();

            Color[] cs = new Color[256];
            {
                try
                {
                    Color[] c = new Color[256];
                    System.IO.StreamReader sr = new System.IO.StreamReader(webDir + @"\wwtweb\colors.map");
                    //System.IO.StreamReader sr = new System.IO.StreamReader(@"colors.map");
                    ArrayList lines = new ArrayList();
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        lines.Add(line);
                        line = sr.ReadLine();
                    }
                    int i = 0;
                    for (i = 0; i < Math.Min(256, lines.Count); i++)
                    {
                        string curC = (string)lines[i];
                        Color temp = Color.FromArgb(int.Parse(curC.Split(' ')[0]), int.Parse(curC.Split(' ')[1]), int.Parse(curC.Split(' ')[2]));
                        c[i] = temp;
                    }
                    for (int j = i; j < 256; j++)
                    {
                        c[j] = Color.White;
                    }
                    cs = c;
                }
                catch (Exception ex)
                {
                    throw new Exception("Invalid ColorMap file.", ex);
                }
            }

	    int MAXITER = 100 + level*100;

            b = new Bitmap(256, 256);
            double x, y, x1, y1, xx, xmin, xmax, ymin, ymax = 0.0;
            int looper, s, z = 0;
            double intigralX, intigralY = 0.0;
            xmin = Sx;
            ymin = Sy;
            xmax = Fx;
            ymax = Fy;
            intigralX = (xmax - xmin) / 256;
            intigralY = (ymax - ymin) / 256;
            x = xmin;
            for (s = 0; s < 256; s++)
            {
                y = ymin;
                for (z = 0; z < 256; z++)
                {
                    x1 = 0;
                    y1 = 0;
                    looper = 0;
                    while (looper < MAXITER && ((x1 * x1) + (y1 * y1)) < 4)
                    {
                        looper++;
                        xx = (x1 * x1) - (y1 * y1) + x;
                        y1 = 2 * x1 * y1 + y;
                        x1 = xx;
                    }
                    double perc = looper / (256.0);
                    int val =  looper % 254;
                    b.SetPixel(s, z, looper == MAXITER ? Color.Black :cs[val]);
                    y += intigralY;
                }
                x += intigralX;
            }
            
            if (level < 32)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                b.Save(filename);
            }
             b.Save(Response.OutputStream, ImageFormat.Png);
             b.Dispose();
       }
  
        Response.End();
	
	%>