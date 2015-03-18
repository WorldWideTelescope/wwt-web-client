using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using WebServices;
using OctSetTest;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;

public partial class WMSToast : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    	
    

  
}

public class WMSImage
{	
	double raMin;
	double decMax;
	double raMax;
	double decMin;
	private const double D2R = 0.017453292519943295;
    	private double decCenter;
    	private FastBitmap fastImage;
    	public Bitmap image;
    	private const double ImageSizeX = 512.0;
    	private const double ImageSizeY = 512.0;
    	private double raCenter;
    	private double scaleX;
    	private double scaleY;
 
    	private double xoff;
    	private double yoff;


	public WMSImage(double raMin, double decMax, double raMax, double decMin)
	{
		this.raMin = raMin;
		this.decMin = decMin;
		this.raMax = raMax;
		this.decMax = decMax;
		scaleX = (this.raMax-this.raMin) / 512;
		scaleY = (this.decMax-this.decMin) / 512;
	}


	public string LoadImage(string url,bool debug)
	{
    		object[] args = new object[] { (raMin-180), decMin, (raMax-180), decMax, 512.0, 512.0, url };
    		string address = string.Format("http://ms.mars.asu.edu/?REQUEST=GetMap&SERVICE=WMS&VERSION=1.1.1&LAYERS={6}&STYLES=&FORMAT=image/png&BGCOLOR=0x000000&TRANSPARENT=FALSE&SRS=JMARS:1&BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={5}&reaspect=false", args);
    	//	string address = string.Format("http://wms.jpl.nasa.gov/wms.cgi?request=GetMap&layers=BMNG&srs=EPSG:4326&format=image/jpeg&styles=&BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={5}", args);
    	//	string address = url+string.Format("BBOX={0},{1},{2},{3}&WIDTH={4}&HEIGHT={5}", args);

		if (debug) return address;
		Stream stream = new WebClient().OpenRead(address);
    		this.image = new Bitmap(stream);
		return address;
	}

	public void Lock()
	{
 		this.fastImage = new FastBitmap(this.image);
 		this.fastImage.LockBitmap();
	}

	public void Unlock()
	{
 	   if (this.fastImage != null)
 	   {
 	       this.fastImage.UnlockBitmap();
 	       this.fastImage.Dispose();
 	       this.fastImage = null;
 	   }
	}

	public PixelData GetPixelDataAtRaDec(Vector2d raDec)
	{
		double x = Math.Max(0,Math.Min((raDec.X - raMin) / this.scaleX,511));
		double y = Math.Max(0,Math.Min(511 -(raDec.Y - decMin) / this.scaleY,511));

			

	    return this.fastImage.GetFilteredPixel(x,y);
	}

 	public string GetPixelDataAtRaDecString(Vector2d raDec)
	{
		double x = (raDec.X - raMin) / this.scaleX;
		double y = (raDec.Y - decMin) / this.scaleY;

		return string.Format("x={0},y={1}, scaleX={2}, scaleY={3}",x,y,scaleX,scaleY);	

	    //return this.fastImage.GetFilteredPixel(x,y);
	}

}



    public class ToastTileMap
    {
        public int X;
        public int Y;
        public int Level;
        public double raMin;
        public double raMax;
        public double decMin;
        public double decMax;

        public ToastTileMap(int level, int x, int y)
        {
            Level = level;
            Y = y;
            X = x;

            int levels = 0;
            Vector3d[,] oldBounds = null;
            backslash = false;
            while (levels <= level)
            {

                if (levels == 0)
                {
                    oldBounds = masterBounds;
                }
                else
                {
                    Vector3d[,] bounds = new Vector3d[3, 3];
                    // equiv : xTemp = (int) (x * Mat.Pow(2,levels-level)) ; note that levels-level < 0

                    int xTemp = (int)(x / Math.Pow(2, level - levels));
                    int yTemp = (int)(y / Math.Pow(2, level - levels));
                    int xIndex = xTemp % 2;
                    int yIndex = yTemp % 2;

                    if (levels == 1)
                    {
                        backslash = xIndex == 1 ^ yIndex == 1;
                    }


                    bounds[0, 0] = oldBounds[xIndex, yIndex];
                    bounds[1, 0] = Vector3d.MidPoint(oldBounds[xIndex, yIndex], oldBounds[xIndex + 1, yIndex]);
                    bounds[2, 0] = oldBounds[xIndex + 1, yIndex];
                    bounds[0, 1] = Vector3d.MidPoint(oldBounds[xIndex, yIndex], oldBounds[xIndex, yIndex + 1]);

                    if (backslash)
                    {
                        bounds[1, 1] = Vector3d.MidPoint(oldBounds[xIndex, yIndex], oldBounds[xIndex + 1, yIndex + 1]);
                    }
                    else
                    {
                        bounds[1, 1] = Vector3d.MidPoint(oldBounds[xIndex + 1, yIndex], oldBounds[xIndex, yIndex + 1]);
                    }

                    bounds[2, 1] = Vector3d.MidPoint(oldBounds[xIndex + 1, yIndex], oldBounds[xIndex + 1, yIndex + 1]);
                    bounds[0, 2] = oldBounds[xIndex, yIndex + 1];
                    bounds[1, 2] = Vector3d.MidPoint(oldBounds[xIndex, yIndex + 1], oldBounds[xIndex + 1, yIndex + 1]);
                    bounds[2, 2] = oldBounds[xIndex + 1, yIndex + 1];
                    oldBounds = bounds;

                }
                levels++;
            }

            Bounds = oldBounds;
            InitGrid();
        }

        static ToastTileMap()
        {
            masterBounds[0, 0] = new Vector3d(0, -1, 0);
            masterBounds[1, 0] = new Vector3d(0, 0, -1);
            masterBounds[2, 0] = new Vector3d(0, -1, 0);
            masterBounds[0, 1] = new Vector3d(1, 0, 0);
            masterBounds[1, 1] = new Vector3d(0, 1, 0);
            masterBounds[2, 1] = new Vector3d(-1, 0, 0);
            masterBounds[0, 2] = new Vector3d(0, -1, 0);
            masterBounds[1, 2] = new Vector3d(0, 0, 1);
            masterBounds[2, 2] = new Vector3d(0, -1, 0);

        }

        protected Vector3d[,] Bounds;
        protected bool backslash = false;
        static protected Vector3d[,] masterBounds = new Vector3d[3, 3];


        Vector2d[,] raDecMap = null;
        int subDivisions = 5;
        float subDivSize = 1.0f / (float)Math.Pow(2, 5);


        void InitGrid()
        {
            List<PositionTexture> vertexList = null;
            List<Triangle> triangleList = null;
            vertexList = new List<PositionTexture>();
            triangleList = new List<Triangle>();

            vertexList.Add(new PositionTexture(Bounds[0, 0], 0, 0));
            vertexList.Add(new PositionTexture(Bounds[1, 0], .5f, 0));
            vertexList.Add(new PositionTexture(Bounds[2, 0], 1, 0));
            vertexList.Add(new PositionTexture(Bounds[0, 1], 0, .5f));
            vertexList.Add(new PositionTexture(Bounds[1, 1], .5f, .5f));
            vertexList.Add(new PositionTexture(Bounds[2, 1], 1, .5f));
            vertexList.Add(new PositionTexture(Bounds[0, 2], 0, 1));
            vertexList.Add(new PositionTexture(Bounds[1, 2], .5f, 1));
            vertexList.Add(new PositionTexture(Bounds[2, 2], 1, 1));

            if (Level == 0)
            {
                triangleList.Add(new Triangle(3, 7, 4));
                triangleList.Add(new Triangle(3, 6, 7));
                triangleList.Add(new Triangle(7, 5, 4));
                triangleList.Add(new Triangle(7, 8, 5));
                triangleList.Add(new Triangle(5, 1, 4));
                triangleList.Add(new Triangle(5, 2, 1));
                triangleList.Add(new Triangle(1, 3, 4));
                triangleList.Add(new Triangle(1, 0, 3));
            }
            else
            {
                if (backslash)
                {
                    triangleList.Add(new Triangle(4, 0, 3));
                    triangleList.Add(new Triangle(4, 1, 0));
                    triangleList.Add(new Triangle(5, 1, 4));
                    triangleList.Add(new Triangle(5, 2, 1));
                    triangleList.Add(new Triangle(3, 7, 4));
                    triangleList.Add(new Triangle(3, 6, 7));
                    triangleList.Add(new Triangle(7, 4, 8));
                    triangleList.Add(new Triangle(4, 7, 8));
                    triangleList.Add(new Triangle(8, 5, 4));


                }
                else
                {

                    triangleList.Add(new Triangle(1, 0, 3));
                    triangleList.Add(new Triangle(1, 3, 4));
                    triangleList.Add(new Triangle(2, 1, 4));
                    triangleList.Add(new Triangle(2, 4, 5));
                    triangleList.Add(new Triangle(6, 4, 3));
                    triangleList.Add(new Triangle(6, 7, 4));
                    triangleList.Add(new Triangle(7, 5, 4));
                    triangleList.Add(new Triangle(8, 5, 7));
                }

            }

            int count = subDivisions;
            subDivSize = 1.0f / (float)Math.Pow(2, subDivisions);
            while (count-- > 1)
            {
                List<Triangle> newList = new List<Triangle>();
                foreach (Triangle tri in triangleList)
                {
                    tri.SubDivide(newList, vertexList);
                }
                triangleList = newList;

            }

            int xCount = 1 + (int)Math.Pow(2, subDivisions);
            int yCount = 1 + (int)Math.Pow(2, subDivisions);

            PositionTexture[,] points = new PositionTexture[xCount, yCount];
            raDecMap = new Vector2d[xCount, yCount];
            foreach (PositionTexture vertex in vertexList)
            {
                int indexX = (int)((vertex.Tu / subDivSize) + .1);
                int indexY = (int)((vertex.Tv / subDivSize) + .1);

                points[indexX, indexY] = vertex;
            }
            for (int y = 0; y < yCount; y++)
            {
                for (int x = 0; x < xCount; x++)
                {
                    raDecMap[x, y] = points[x, y].Position.ToRaDec((y == 0 || x == 32) & !backslash);
                }
            }

            if (Level == 0)
            {
                raMin = 0;
                raMax = 360;
                decMin = -90;
                decMax = 90;
            }
            else
            {
                raMin = Math.Min(Math.Min(raDecMap[0, 0].X, raDecMap[0, yCount - 1].X), Math.Min(raDecMap[xCount - 1, 0].X, raDecMap[xCount - 1, yCount - 1].X));
                raMax = Math.Max(Math.Max(raDecMap[0, 0].X, raDecMap[0, yCount - 1].X), Math.Max(raDecMap[xCount - 1, 0].X, raDecMap[xCount - 1, yCount - 1].X));
                decMin = Math.Min(Math.Min(raDecMap[0, 0].Y, raDecMap[0, yCount - 1].Y), Math.Min(raDecMap[xCount - 1, 0].Y, raDecMap[xCount - 1, yCount - 1].Y));
                decMax = Math.Max(Math.Max(raDecMap[0, 0].Y, raDecMap[0, yCount - 1].Y), Math.Max(raDecMap[xCount - 1, 0].Y, raDecMap[xCount - 1, yCount - 1].Y));
 		if (Math.Abs((double) (this.raMax - this.raMin)) > 180.0)
      		{
         	   this.raMin = this.raMax;
         	   this.raMax = 360.0;
        	}

            }
        }
        public Vector2d PointToRaDec(double x, double y ) // point is between 0 and 1 inclusive
        {
	    Vector2d point = new Vector2d(x,y);	
            int indexX = (int)(point.X / subDivSize);
            int indexY = (int)(point.Y / subDivSize);

            if (indexX > ((int)Math.Pow(2, subDivisions) - 1))
            {
                indexX = ((int)Math.Pow(2, subDivisions) - 1);
            }
            if (indexY > ((int)Math.Pow(2, subDivisions) - 1))
            {
                indexY = ((int)Math.Pow(2, subDivisions) - 1);
            }
            double xDist = (point.X - ((double)indexX * subDivSize)) / subDivSize;
            double yDist = (point.Y - ((double)indexY * subDivSize)) / subDivSize;

            Vector2d interpolatedTop = Vector2d.Lerp(raDecMap[indexX, indexY], raDecMap[indexX + 1, indexY], xDist);
            Vector2d interpolatedBottom = Vector2d.Lerp(raDecMap[indexX, indexY + 1], raDecMap[indexX + 1, indexY + 1], xDist);
            Vector2d result = Vector2d.Lerp(interpolatedTop, interpolatedBottom, yDist);


            return result;
        }
	public Vector2d PointToRaDec(Vector2d point ) // point is between 0 and 1 inclusive
        {

            int indexX = (int)(point.X / subDivSize);
            int indexY = (int)(point.Y / subDivSize);

            if (indexX > ((int)Math.Pow(2, subDivisions) - 1))
            {
                indexX = ((int)Math.Pow(2, subDivisions) - 1);
            }
            if (indexY > ((int)Math.Pow(2, subDivisions) - 1))
            {
                indexY = ((int)Math.Pow(2, subDivisions) - 1);
            }
            double xDist = (point.X - ((double)indexX * subDivSize)) / subDivSize;
            double yDist = (point.Y - ((double)indexY * subDivSize)) / subDivSize;

            Vector2d interpolatedTop = Vector2d.Lerp(raDecMap[indexX, indexY], raDecMap[indexX + 1, indexY], xDist);
            Vector2d interpolatedBottom = Vector2d.Lerp(raDecMap[indexX, indexY + 1], raDecMap[indexX + 1, indexY + 1], xDist);
            Vector2d result = Vector2d.Lerp(interpolatedTop, interpolatedBottom, yDist);


            return result;
        }

    }

    // Summary:
    //     Describes a custom vertex format structure that contains position and one
    //     set of texture coordinates.
    public struct PositionTexture
    {
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public double Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public double Tv;
        //
        // Summary:
        //     Retrieves or sets the x component of the position.
        public double X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public double Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public double Z;

        //
        // Summary:
        //     Initializes a new instance of the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3d object that contains the vertex position.
        //
        //   u:
        //     Floating-point value that represents the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured.#ctor()
        //     component of the texture coordinate.
        public PositionTexture(Vector3d pos, double u, double v)
        {
            Tu = u;
            Tv = v;
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }
        //
        // Summary:
        //     Initializes a new instance of the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured
        //     class.
        //
        // Parameters:
        //   xvalue:
        //     Floating-point value that represents the x coordinate of the position.
        //
        //   yvalue:
        //     Floating-point value that represents the y coordinate of the position.
        //
        //   zvalue:
        //     Floating-point value that represents the z coordinate of the position.
        //
        //   u:
        //     Floating-point value that represents the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the Microsoft.DirectX.Direct3D.CustomVertex.PositionTextured.#ctor()
        //     component of the texture coordinate.
        public PositionTexture(double xvalue, double yvalue, double zvalue, double u, double v)
        {
            Tu = u;
            Tv = v;
            X = xvalue;
            Y = yvalue;
            Z = zvalue;
        }

        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position 
        {
            get
            {
                return new Vector3d(X,Y,Z);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}", X, Y, Z, Tu, Tv);
        }



    }
   // Summary:
    //     Describes and manipulates a vector in three-dimensional (3-D) space.
    [Serializable]
    public struct Vector3d
    {
        // Summary:
        //     Retrieves or sets the x component of a 3-D vector.
        public double X;
        //
        // Summary:
        //     Retrieves or sets the y component of a 3-D vector.
        public double Y;
        //
        // Summary:
        //     Retrieves or sets the z component of a 3-D vector.
        public double Z;

        //
        // Summary:
        //     Initializes a new instance of the Microsoft.DirectX.Vector3d class.
        //
        // Parameters:
        //   valueX:
        //     Initial Microsoft.DirectX.Vector3d.X value.
        //
        //   valueY:
        //     Initial Microsoft.DirectX.Vector3d.Y value.
        //
        //   valueZ:
        //     Initial Microsoft.DirectX.Vector3d.Z value.
        public Vector3d(double valueX, double valueY, double valueZ)
        {
            X = valueX;
            Y = valueY;
            Z = valueZ;
        }
        public Vector3d(Vector3d value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
        }
        // Summary:
        //     Negates the vector.
        //
        // Parameters:
        //   vec:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     The Microsoft.DirectX.Vector3d structure that is the result of the operation.
        public static Vector3d operator -(Vector3d vec)
        {
            Vector3d result;
            result.X = - vec.X;
            result.Y = - vec.Y;
            result.Z = - vec.Z;
            return result;
        }
        //
        // Summary:
        //     Subtracts two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     The Microsoft.DirectX.Vector3d structure to the left of the subtraction operator.
        //
        //   right:
        //     The Microsoft.DirectX.Vector3d structure to the right of the subtraction operator.
        //
        // Returns:
        //     Resulting Microsoft.DirectX.Vector3d structure.
        public static Vector3d operator -(Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X - right.X, left.Y - right.Y, left.Z - left.Z);
        }
        //
        // Summary:
        //     Compares the current instance of a class to another instance to determine
        //     whether they are different.
        //
        // Parameters:
        //   left:
        //     The Microsoft.DirectX.Vector3d structure to the left of the inequality operator.
        //
        //   right:
        //     The Microsoft.DirectX.Vector3d structure to the right of the inequality operator.
        //
        // Returns:
        //     Value that is true if the objects are different, or false if they are the
        //     same.
        public static bool operator !=(Vector3d left, Vector3d right)
        {
            return (left.X != right.X || left.Y != right.Y || left.Z != right.Z);
        }
        //
        // Summary:
        //     Determines the product of a single value and a 3-D vector.
        //
        // Parameters:
        //   right:
        //     Source System.Single structure.
        //
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the product of the Microsoft.DirectX.Vector3d.op_Multiply()
        //     and Microsoft.DirectX.Vector3d.op_Multiply() parameters.
        //public static Vector3d operator *(double right, Vector3d left);
        //
        // Summary:
        //     Determines the product of a single value and a 3-D vector.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source System.Single structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the product of the Microsoft.DirectX.Vector3d.op_Multiply()
        //     and Microsoft.DirectX.Vector3d.op_Multiply() parameters.
        //public static Vector3d operator *(Vector3d left, double right);
        //
        // Summary:
        //     Adds two vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that contains the sum of the parameters.
        //public static Vector3d operator +(Vector3d left, Vector3d right);
        //
        // Summary:
        //     Compares the current instance of a class to another instance to determine
        //     whether they are the same.
        //
        // Parameters:
        //   left:
        //     The Microsoft.DirectX.Vector3d structure to the left of the equality operator.
        //
        //   right:
        //     The Microsoft.DirectX.Vector3d structure to the right of the equality operator.
        //
        // Returns:
        //     Value that is true if the objects are the same, or false if they are different.
        public static bool operator ==(Vector3d left, Vector3d right)
        {
            return (left.X == right.X || left.Y == right.Y || left.Z == right.Z);
        }
        public static Vector3d MidPoint(Vector3d left, Vector3d right)
        {
            Vector3d result = new Vector3d((left.X + right.X) / 2, (left.Y + right.Y) / 2, (left.Z + right.Z) / 2);
            result.Normalize();
            return result;
        }
        // Summary:
        //     Retrieves an empty 3-D vector.
        public static Vector3d Empty 
        {
            get
            {
                return new Vector3d(0, 0, 0);
            }
        }

        // Summary:
        //     Adds two 3-D vectors.
        //
        // Parameters:
        //   source:
        public void Add(Vector3d source)
        {
            X += source.X;
            Y += source.Y;
            Z += source.Z;
        }

        //
        // Summary:
        //     Adds two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d.
        //
        // Returns:
        //     Sum of the two Microsoft.DirectX.Vector3d structures.
        public static Vector3d Add(Vector3d left, Vector3d right)
        {
            return new Vector3d(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        //
        // Summary:
        //     Returns a point in barycentric coordinates, using specified 3-D vectors.
        //
        // Parameters:
        //   v1:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   v2:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   v3:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   f:
        //     Weighting factor. See Remarks.
        //
        //   g:
        //     Weighting factor. See Remarks.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure in barycentric coordinates.
        //public static Vector3d BaryCentric(Vector3d v1, Vector3d v2, Vector3d v3, double f, double g);
        //
        // Summary:
        //     Performs a Catmull-Rom interpolation using specified 3-D vectors.
        //
        // Parameters:
        //   position1:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   position2:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   position3:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   position4:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   weightingFactor:
        //     Weighting factor. See Remarks.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the result of the Catmull-Rom
        //     interpolation.
        //public static Vector3d CatmullRom(Vector3d position1, Vector3d position2, Vector3d position3, Vector3d position4, double weightingFactor)
        //{
        //}
        //
        // Summary:
        //     Determines the cross product of two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the cross product of two 3-D
        //     vectors.
        public static Vector3d Cross(Vector3d left, Vector3d right)
        {
            return new Vector3d(
                  left.Y * right.Z - left.Z * right.Y,
                  left.Z * right.X - left.X * right.Z,
                  left.X * right.Y - left.Y * right.X);

        }
        //
        // Summary:
        //     Determines the dot product of two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A System.Single value that is the dot product.
        public static double Dot(Vector3d left, Vector3d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }
        //
        // Summary:
        //     Returns a value that indicates whether the current instance is equal to a
        //     specified object.
        //
        // Parameters:
        //   compare:
        //     Object with which to make the comparison.
        //
        // Returns:
        //     Value that is true if the current instance is equal to the specified object,
        //     or false if it is not.
        public override bool Equals(object compare)
        {
            Vector3d comp = (Vector3d)compare;
            return this.X == comp.X && this.Y == comp.Y && this.Z == comp.Z;
        }
        //
        // Summary:
        //     Returns the hash code for the current instance.
        //
        // Returns:
        //     Hash code for the instance.
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }
        //
        // Summary:
        //     Performs a Hermite spline interpolation using the specified 3-D vectors.
        //
        // Parameters:
        //   position:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   tangent:
        //     Source Microsoft.DirectX.Vector3d structure that is a tangent vector.
        //
        //   position2:
        //     Source Microsoft.DirectX.Vector3d structure that is a position vector.
        //
        //   tangent2:
        //     Source Microsoft.DirectX.Vector3d structure that is a tangent vector.
        //
        //   weightingFactor:
        //     Weighting factor. See Remarks.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the result of the Hermite spline
        //     interpolation.
        //public static Vector3d Hermite(Vector3d position, Vector3d tangent, Vector3d position2, Vector3d tangent2, double weightingFactor);
        //
        // Summary:
        //     Returns the length of a 3-D vector.
        //
        // Returns:
        //     A System.Single value that contains the vector's length.
        public double Length()
        {
            return System.Math.Sqrt(X * X + Y * Y + Z * Z);
        }
        //
        // Summary:
        //     Returns the length of a 3-D vector.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A System.Single value that contains the vector's length.
        public static double Length(Vector3d source)
        {
            return System.Math.Sqrt(source.X * source.X + source.Y * source.Y + source.Z * source.Z);

        }
        //
        // Summary:
        //     Returns the square of the length of a 3-D vector.
        //
        // Returns:
        //     A System.Single value that contains the vector's squared length.
        public double LengthSq()
        {
            return X * X + Y * Y + Z * Z;
        }
        //
        // Summary:
        //     Returns the square of the length of a 3-D vector.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A System.Single value that contains the vector's squared length.
        public static double LengthSq(Vector3d source)
        {
            return source.X * source.X + source.Y * source.Y + source.Z * source.Z;
        }

        //
        // Summary:
        //     Performs a linear interpolation between two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   interpolater:
        //     Parameter that linearly interpolates between the vectors.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the result of the linear interpolation.
        public static Vector3d Lerp(Vector3d left, Vector3d right, double interpolater)
        {
            return new Vector3d(
                left.X * (1.0 - interpolater) + right.X * interpolater,
                left.Y * (1.0 - interpolater) + right.Y * interpolater,
                left.Z * (1.0 - interpolater) + right.Z * interpolater);

        }
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the largest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //public void Maximize(Vector3d source);
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the largest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is made up of the largest components
        //     of the two vectors.
        //public static Vector3d Maximize(Vector3d left, Vector3d right);
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the smallest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //public void Minimize(Vector3d source);
        //
        // Summary:
        //     Returns a 3-D vector that is made up of the smallest components of two 3-D
        //     vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is made up of the smallest components
        //     of the two vectors.
        //public static Vector3d Minimize(Vector3d left, Vector3d right);
        //
        // Summary:
        //     Multiplies a 3-D vector by a System.Single value.
        //
        // Parameters:
        //   s:
        //     Source System.Single value used as a multiplier.
        public void Multiply(double s)
        {
            X *= s;
            Y *= s;
            Z *= s;
        }
        //
        // Summary:
        //     Multiplies a 3-D vector by a System.Single value.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   f:
        //     Source System.Single value used as a multiplier.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is multiplied by the System.Single
        //     value.
        public static Vector3d Multiply(Vector3d source, double f)
        {
            Vector3d result = new Vector3d(source);
            result.Multiply(f);
            return result;
        }
        //
        // Summary:
        //     Returns the normalized version of a 3-D vector.
        public void Normalize()
        {
            // Vector3.Length property is under length section
            double length = this.Length();
            if (length!=0)
            {
                X /= length;
                Y /= length;
                Z /= length;
            }
        }

        //
        // Summary:
        //     Scales a 3-D vector.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure.
        //
        //   scalingFactor:
        //     Scaling value.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the scaled vector.
        public static Vector3d Scale(Vector3d source, double scalingFactor)
        {
            Vector3d result = source;
            result.Multiply(scalingFactor);
            return result;
        }
        //
        // Summary:
        //     Subtracts two 3-D vectors.
        //
        // Parameters:
        //   source:
        //     Source Microsoft.DirectX.Vector3d structure to subtract from the current instance.
        public void Subtract(Vector3d source)
        {
            this.X -= source.X;
            this.Y -= source.Y;
            this.Z -= source.Z;

        }
        //
        // Summary:
        //     Subtracts two 3-D vectors.
        //
        // Parameters:
        //   left:
        //     Source Microsoft.DirectX.Vector3d structure to the left of the subtraction
        //     operator.
        //
        //   right:
        //     Source Microsoft.DirectX.Vector3d structure to the right of the subtraction
        //     operator.
        //
        // Returns:
        //     A Microsoft.DirectX.Vector3d structure that is the result of the operation.
        public static Vector3d Subtract(Vector3d left, Vector3d right)
        {
            Vector3d result = left;
            result.Subtract(right);
            return result;
        }
        //
        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", X, Y, Z);
        }
        public Vector2d ToSpherical()
        {

            double ascention;
            double declination;

            double radius = Math.Sqrt(X * X + Y * Y + Z * Z);
            double XZ = Math.Sqrt(X * X + Z * Z);
            declination = Math.Asin(Y / radius);
            if (XZ == 0)
            {
                ascention = 0;
            }
            else if (0 <= X)
            {
                ascention = Math.Asin(Z / XZ);
            }
            else
            {
                ascention = Math.PI - Math.Asin(Z / XZ);
            }

            //if (vector.Z < 0)
            //{
            //    ascention = ascention - Math.PI;
            //}
   // 0 -1.0         return new Vector2d((((ascention + Math.PI) / (2.0 * Math.PI)) % 1.0f), ((declination + (Math.PI / 2.0)) / (Math.PI)));
            return new Vector2d((((ascention + Math.PI) % (2.0 * Math.PI))), ((declination + (Math.PI / 2.0))));

        }
        public Vector2d ToRaDec(bool edge)
        {
            Vector2d point = ToSpherical();
            point.X = point.X / Math.PI * 180;
	    if (edge && point.X == 0)
            {
                point.X = 360;
            }
            point.Y = (point.Y / Math.PI * 180)-90;
            return point;
        }

    }
    public struct Vector2d
    {
        public double X;
        public double Y;
        public Vector2d(double x, double y)
        {
            X = x;
            Y = y;
        }
        public static Vector2d Lerp(Vector2d left, Vector2d right, double interpolater)
        {
            if (Math.Abs(left.X - right.X) > 180)
            {
                if (left.X > right.X)
                {
                    right.X += 360;
                }
                else
                {
                    left.X += 360;
                }
            }
            return new Vector2d(left.X * (1 - interpolater) + right.X * interpolater, left.Y * (1 - interpolater) + right.Y * interpolater);

        }
    }
    class Triangle
    {
        // Vertex Indexies
        public int A;
        public int B;
        public int C;

        public Triangle(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Triangle()
        {
            A = -1;
            B = -1;
            C = -1;
        }

        public void SubDivide(List<Triangle> triList, List<PositionTexture> vertexList)
        {
            Vector3d a1 = Vector3d.Lerp(vertexList[B].Position, vertexList[C].Position, .5f);
            Vector3d b1 = Vector3d.Lerp(vertexList[C].Position, vertexList[A].Position, .5f);
            Vector3d c1 = Vector3d.Lerp(vertexList[A].Position, vertexList[B].Position, .5f);

            Vector2d a1uv = Vector2d.Lerp(new Vector2d(vertexList[B].Tu, vertexList[B].Tv), new Vector2d(vertexList[C].Tu, vertexList[C].Tv), .5);
            Vector2d b1uv = Vector2d.Lerp(new Vector2d(vertexList[C].Tu, vertexList[C].Tv), new Vector2d(vertexList[A].Tu, vertexList[A].Tv), .5);
            Vector2d c1uv = Vector2d.Lerp(new Vector2d(vertexList[A].Tu, vertexList[A].Tv), new Vector2d(vertexList[B].Tu, vertexList[B].Tv), .5);

            a1.Normalize();
            b1.Normalize();
            c1.Normalize();

            int aIndex = vertexList.Count;
            int bIndex = vertexList.Count + 1;
            int cIndex = vertexList.Count + 2;

            vertexList.Add(new PositionTexture(a1, a1uv.X, a1uv.Y));
            vertexList.Add(new PositionTexture(b1, b1uv.X, b1uv.Y));
            vertexList.Add(new PositionTexture(c1, c1uv.X, c1uv.Y));

            triList.Add(new Triangle(A, cIndex, bIndex));
            triList.Add(new Triangle(B, aIndex, cIndex));
            triList.Add(new Triangle(C, bIndex, aIndex));
            triList.Add(new Triangle(aIndex, bIndex, cIndex));
        }
    }
  public struct PixelData
    {
        public byte blue;
        public byte green;
        public byte red;
        public byte alpha;

        public PixelData(int r, int g, int b, int a) // values between 0 and 255
        {
            blue = (byte)(b >= 255 ? 255 : (b < 0 ? 0 : b));
            red = (byte)(r >= 255 ? 255 : (r < 0 ? 0 : r));
            green = (byte)(g >= 255 ? 255 : (g < 0 ? 0 : g));
            alpha = (byte)(a >= 255 ? 255 : (a < 0 ? 0 : a));
        }
    }

    public unsafe class FastBitmapEnumerator : IDisposable
    {
        int x;
        int y;
        FastBitmap fastBitmap;
        PixelData* pCurrentPixel;
        bool locked;

        public FastBitmapEnumerator(FastBitmap fastBitmap)
        {
            fastBitmap.LockBitmap();
            locked = true;
            this.fastBitmap = fastBitmap;
            x = -1;
            y = 0;
            pCurrentPixel = fastBitmap[x, y];
        }

        public void Dispose()
        {
            if (locked)
            {
                fastBitmap.UnlockBitmap();
            }
        }

        public bool MoveNext()
        {
            x++;
            pCurrentPixel++;
            if (x == fastBitmap.Size.X)
            {
                y++;
                if (y == fastBitmap.Size.Y)
                {
                    return false;
                }
                else
                {
                    x = 0;
                    pCurrentPixel = fastBitmap[0, y];
                    //Debug.WriteLine(String.Format("{0}", pCurrentPixel - fastBitmap[0, 0]));
                }
            }
            return true;
        }

        public PixelData* Current
        {
            get
            {
                return pCurrentPixel;
            }
        }


    }

    /// <summary>
    /// A bitmap class that allows fast x, y access 
    /// </summary>
    public unsafe class FastBitmap
    {
        Bitmap bitmap;

        // three elements used for MakeGreyUnsafe
        int width;
        BitmapData bitmapData = null;
        Byte* pBase = null;
        PixelData* pCurrentPixel = null;
        int xLocation;
        int yLocation;
        Point size;
        internal bool locked = false;

        /// <summary>
        /// Create an instance from an existing bitmap
        /// </summary>
        /// <param name="bitmap">The bitmap</param>
        public FastBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;

            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref unit);

            size = new Point((int)bounds.Width, (int)bounds.Height);
        }

        /// <summary>
        /// Save the bitmap to a file
        /// </summary>
        /// <param name="filename">Filename to save the bitmap to</param>
        public void Save(string filename)
        {
            bitmap.Save(filename, ImageFormat.Jpeg);
        }

        public void Dispose()
        {
            bitmap.Dispose();
        }

        /// <summary>
        /// Size of the bitmap in pixels
        /// </summary>
        public Point Size
        {
            get
            {
                return size;
            }
        }

        /// <summary>
        /// The Bitmap object wrapped by this instance
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                return (bitmap);
            }
        }

        /// <summary>
        /// Start at the beginning of the bitmap
        /// </summary>
        public void InitCurrentPixel()
        {
            LockBitmap();
            //if (pBase == null)
            //{
            //		throw new InvalidOperationException("Bitmap must be locked before calling InitCurrentPixel()");
            //		}
            pCurrentPixel = (PixelData*)pBase;
        }

        /// <summary>
        /// Return the next pixel
        /// </summary>
        /// <returns>The next pixel, or null if done</returns>
        public PixelData* GetNextPixel()
        {
            PixelData* pReturnPixel = pCurrentPixel;
            if (xLocation == size.X)
            {
                xLocation = 0;
                yLocation++;
                if (yLocation == size.Y)
                {
                    UnlockBitmap();
                    return null;
                }
                else
                {
                    pCurrentPixel = this[0, yLocation];
                }
            }
            else
            {
                xLocation++;
                pCurrentPixel++;
            }
            return pReturnPixel;
        }

        /// <summary>
        /// Get the pixel data at a specific x and y location
        /// </summary>
        public PixelData* this[int x, int y]
        {
            get
            {
                return (PixelData*)(pBase + y * width + x * sizeof(PixelData));
            }
        }

        public FastBitmapEnumerator GetEnumerator()
        {
            return new FastBitmapEnumerator(this);
        }

        /// <summary>
        /// Lock the bitmap. 
        /// </summary>
        public void LockBitmap()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X,
                (int)boundsF.Y,
                (int)boundsF.Width,
                (int)boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            width = (int)boundsF.Width * sizeof(PixelData);
            if (width % 4 != 0)
            {
                width = 4 * (width / 4 + 1);
            }

            bitmapData =
                bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            pBase = (Byte*)bitmapData.Scan0.ToPointer();
            locked = true;
        }

        /// <summary>
        /// Unlock the bitmap
        /// </summary>
        public void UnlockBitmap()
        {
            bitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
            locked = false;
        }

        public PixelData GetFilteredPixel(double xd, double yd)
        {
            int x = (int)(xd);

            double xr = xd - (double)x;

            int y = (int)(yd);

            double yr = yd - (double)y;

            if (x < 0 || x > (bitmap.Width - 1) || y < 0 || y > (bitmap.Height - 1))
            {
                return new PixelData();
            }

            int stepX = 1;
            int stepY = 1;

            if (x == bitmap.Width - 1)
            {
                stepX = 0;
            }
            if (y == bitmap.Height - 1)
            {
                stepY = 0;
            }

            PixelData* tl = this[x, y];
            PixelData* tr = this[x + stepX, y];
            PixelData* bl = this[x, y + stepY];
            PixelData* br = this[x + stepX, y + stepY];

            PixelData result;

            result.alpha = (byte)((((((double)tl->alpha * (1.0 - xr)) + ((double)tr->alpha * xr))) * (1.0 - yr)) +
                             (((((double)bl->alpha * (1.0 - xr)) + ((double)br->alpha * xr))) * yr));
            result.red = (byte)((((((double)tl->red * (1.0 - xr)) + ((double)tr->red * xr))) * (1.0 - yr)) +
                             (((((double)bl->red * (1.0 - xr)) + ((double)br->red * xr))) * yr));
            result.green = (byte)((((((double)tl->green * (1.0 - xr)) + ((double)tr->green * xr))) * (1.0 - yr)) +
                             (((((double)bl->green * (1.0 - xr)) + ((double)br->green * xr))) * yr));
            result.blue = (byte)((((((double)tl->blue * (1.0 - xr)) + ((double)tr->blue * xr))) * (1.0 - yr)) +
                             (((((double)bl->blue * (1.0 - xr)) + ((double)br->blue * xr))) * yr));


            return result;
        }


    }

