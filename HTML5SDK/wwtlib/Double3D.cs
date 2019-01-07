using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    // Summary:
    //     Describes a custom vertex format structure that contains position and one
    //     set of texture coordinates.
    public enum LocationHint { Slash = 0, Backslash = 1, Top = 2 };

    public class PositionTexture
    {
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public double Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public double Tv;
        ////
        //// Summary:
        ////     Retrieves or sets the x component of the position.
        //public double X;
        ////
        //// Summary:
        ////     Retrieves or sets the y component of the position.
        //public double Y;
        ////
        //// Summary:
        ////     Retrieves or sets the z component of the position.
        //public double Z;

        public PositionTexture()
        {
            Position = new Vector3d();
        }

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
        public static PositionTexture CreatePos(Vector3d pos, double u, double v)
        {
            PositionTexture temp = new PositionTexture();

            temp.Tu = u*Tile.uvMultiple;
            temp.Tv = v*Tile.uvMultiple;
            temp.Position = pos;
           
            return temp;
        }
        
        public static PositionTexture CreatePosRaw(Vector3d pos, double u, double v)
        {
            PositionTexture temp = new PositionTexture();

            temp.Tu = u;
            temp.Tv = v;
            temp.Position = pos;
           
            return temp;
        }

        public static PositionTexture CreatePosSize(Vector3d pos, double u, double v, double width, double height)
        {
            PositionTexture temp = new PositionTexture();

            temp.Tu = u * width;
            temp.Tv = v * height;
            temp.Position = pos;

            return temp;
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
        public static PositionTexture Create(double xvalue, double yvalue, double zvalue, double u, double v)
        {
            PositionTexture temp = new PositionTexture();
            temp.Position = Vector3d.Create(xvalue, yvalue, zvalue);
            temp.Tu = u*Tile.uvMultiple;
            temp.Tv = v*Tile.uvMultiple;
            return temp;
        }

        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position;
        //{
        //    get
        //    {
        //        return Vector3d.Create(X, Y, Z);
        //    }
        //    set
        //    {
        //        X = value.X;
        //        Y = value.Y;
        //        Z = value.Z;
        //    }
        //}

        public PositionTexture Copy()
        {
            PositionTexture temp = new PositionTexture();
            temp.Position = Vector3d.MakeCopy(this.Position);
            temp.Tu = this.Tu;
            temp.Tv = this.Tv;
            return temp;
        }

        //public PositionNormalTexturedX2 PositionNormalTextured(Vector3d center, bool backslash)
        //{


        //    Coordinates latLng = Coordinates.CartesianToSpherical2(this.Position);
        //    //      latLng.Lng += 90;
        //    if (latLng.Lng < -180)
        //    {
        //        latLng.Lng += 360;
        //    }
        //    if (latLng.Lng > 180)
        //    {
        //        latLng.Lng -= 360;
        //    }
        //    if (latLng.Lng == -180 && !backslash)
        //    {
        //        latLng.Lng = 180;
        //    }
        //    if (latLng.Lng == 180 && backslash)
        //    {
        //        latLng.Lng = -180;
        //    }
        //    PositionNormalTexturedX2 pnt = new PositionNormalTexturedX2();

        //    pnt.X = (float)(X - center.X);
        //    pnt.Y = (float)(Y - center.Y);
        //    pnt.Z = (float)(Z - center.Z);
        //    pnt.Tu = (float)Tu;
        //    pnt.Tv = (float)Tv;
        //    pnt.Lng = latLng.Lng;
        //    pnt.Lat = latLng.Lat;
        //    pnt.Normal = Position;
        //    return pnt;

        //}

        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}", Position.X, Position.Y, Position.Z, Tu, Tv);
        }
    }


    public class PositionColoredTextured
    {
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public double Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public double Tv;
        ////
        //// Summary:
        ////     Retrieves or sets the x component of the position.
        //public double X;
        ////
        //// Summary:
        ////     Retrieves or sets the y component of the position.
        //public double Y;
        ////
        //// Summary:
        ////     Retrieves or sets the z component of the position.
        //public double Z;

        public PositionColoredTextured()
        {
            Position = new Vector3d();
        }

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
        public static PositionColoredTextured CreatePos(Vector3d pos, double u, double v)
        {
            PositionColoredTextured temp = new PositionColoredTextured();

            temp.Tu = u * Tile.uvMultiple;
            temp.Tv = v * Tile.uvMultiple;
            temp.Position = pos;

            return temp;
        }

        public static PositionColoredTextured CreatePosRaw(Vector3d pos, double u, double v)
        {
            PositionColoredTextured temp = new PositionColoredTextured();

            temp.Tu = u;
            temp.Tv = v;
            temp.Position = pos;

            return temp;
        }

        public static PositionColoredTextured CreatePosSize(Vector3d pos, double u, double v, double width, double height)
        {
            PositionColoredTextured temp = new PositionColoredTextured();

            temp.Tu = u * width;
            temp.Tv = v * height;
            temp.Position = pos;

            return temp;
        }

        public Color Color = new Color();

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
        public static PositionTexture Create(double xvalue, double yvalue, double zvalue, double u, double v)
        {
            PositionTexture temp = new PositionTexture();
            temp.Position = Vector3d.Create(xvalue, yvalue, zvalue);
            temp.Tu = u * Tile.uvMultiple;
            temp.Tv = v * Tile.uvMultiple;
            return temp;
        }

        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position;
        //{
        //    get
        //    {
        //        return Vector3d.Create(X, Y, Z);
        //    }
        //    set
        //    {
        //        X = value.X;
        //        Y = value.Y;
        //        Z = value.Z;
        //    }
        //}

        public PositionTexture Copy()
        {
            PositionTexture temp = new PositionTexture();
            temp.Position = Vector3d.MakeCopy(this.Position);
            temp.Tu = this.Tu;
            temp.Tv = this.Tv;
            return temp;
        }


        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}", Position.X, Position.Y, Position.Z, Tu, Tv);
        }



    }

    public class PositionColored
    {

     
        public PositionColored (Vector3d pos, Color color)
        {     
               
            Color = color.Clone();
            Position = pos.Copy();
        }
 
        public Color Color = new Color();
        public Vector3d Position;

        public PositionColored Copy()
        {
            PositionColored temp = new PositionColored(this.Position, this.Color);
            return temp;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}", Position.X, Position.Y, Position.Z, Color.ToString());
        }
    }

    // Summary:
    //    Custom vertex format with position, normal, texture coordinate, and tangent vector. The
    //    tangent vector is stored in the second texture coordinate.
    public class PositionNormalTexturedTangent
    {
        // Summary:
        //     Retrieves the Microsoft.DirectX.Direct3D.VertexFormats for the current custom
        //     vertex.
        //public static  VertexFormats Format = VertexFormats.Position |
        //                                      VertexFormats.Normal   |
        //                                      VertexFormats.Texture2 |
        //                                      VertexTextureCoordinate.Size3(1);
        public double X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public double Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public double Z;
        // Summary:
        //     Retrieves or sets the nx component of the vertex normal.
        public double Nx;
        //
        // Summary:
        //     Retrieves or sets the ny component of the vertex normal.
        public double Ny;
        //
        // Summary:
        //     Retrieves or sets the nz component of the vertex normal.
        public double Nz;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public double Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public double Tv;
        //
        // Summary:
        //     Retrieves or sets the x component of the tangent vector
        public double Tanx;
        //
        // Summary:
        //     Retrieves or sets the y component of the tangent vector
        public double Tany;
        //
        // Summary:
        //     Retrieves or sets the z component of the tangent vector
        public double Tanz;

        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedTangent
        //     class.
        //
        public PositionNormalTexturedTangent(Vector3d position, Vector3d normal, Vector2d texCoord, Vector3d tangent)
        {
            X = position.X;
            Y = position.Y;
            Z = position.Z;
            Nx = normal.X;
            Ny = normal.Y;
            Nz = normal.Z;
            Tu = texCoord.X;
            Tv = texCoord.Y;
            Tanx = tangent.X;
            Tany = tangent.Y;
            Tanz = tangent.Z;
        }

        // Summary:
        //     Retrieves or sets the vertex normal data.
        public Vector3d Normal
        {
            get
            {
                return Vector3d.Create(Nx, Ny, Nz);
            }
            set
            {
                Nx = value.X;
                Ny = value.Y;
                Nz = value.Z;
            }
        }

        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position
        {
            get
            {
                return Vector3d.Create(X, Y, Z);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        // Summary:
        //     Retrieves or sets the texture coordinate.
        public Vector2d TexCoord
        {
            get
            {
                return Vector2d.Create(Tu, Tv);
            }
            set
            {
                Tu = value.X;
                Tv = value.Y;
            }
        }

        // Summary:
        //     Retrieves or sets the vertex tangent.
        public Vector3d Tangent
        {
            get
            {
                return Vector3d.Create(Tanx, Tany, Tanz);
            }
            set
            {
                Tanx = value.X;
                Tany = value.Y;
                Tanz = value.Z;
            }
        }



        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return string.Format(
                "X={0}, Y={1}, Z={2}, Nx={3}, Ny={4}, Nz={5}, U={6}, V={7}, TanX={8}, TanY={9}, TanZ={10}",
                X, Y, Z, Nx, Ny, Nz, Tu, Tv, Tanx, Tany, Tanz
                );
        }
    }


    // Summary:
    //     Describes and manipulates a vector in three-dimensional (3-D) space.

    public class Vector3d
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
        public static Vector3d Create(double valueX, double valueY, double valueZ)
        {
            Vector3d temp = new Vector3d();
            temp.X = valueX;
            temp.Y = valueY;
            temp.Z = valueZ;
            return temp;
        }

        public void Set(double valueX, double valueY, double valueZ)
        {
            X = valueX;
            Y = valueY;
            Z = valueZ;
        }

        public static Vector3d MakeCopy(Vector3d value)
        {
            Vector3d temp = new Vector3d();
            temp.X = value.X;
            temp.Y = value.Y;
            temp.Z = value.Z;
            return temp;
        }

        public Vector3d Copy()
        {
            Vector3d temp = new Vector3d();
            temp.X = X;
            temp.Y = Y;
            temp.Z = Z;
            return temp;
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
        public static Vector3d Negate(Vector3d vec)
        {
            return Vector3d.Create(-vec.X, -vec.Y, -vec.Z);
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
        //public static Vector3d Subtract(Vector3d left, Vector3d right)
        //{
        //    return Vector3d.Create(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        //}
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
        //public static bool operator !=(Vector3d left, Vector3d right)
        //{
        //    return (left.X != right.X || left.Y != right.Y || left.Z != right.Z);
        //}
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
        //public static bool operator ==(Vector3d left, Vector3d right)
        //{
        //    return (left.X == right.X || left.Y == right.Y || left.Z == right.Z);
        //}
        public static Vector3d MidPoint(Vector3d left, Vector3d right)
        {
            Vector3d result = Vector3d.Create((left.X + right.X) / 2, (left.Y + right.Y) / 2, (left.Z + right.Z) / 2);
            result.Normalize();
            return result;
        }
        public static Vector3d MidPointByLength(Vector3d left, Vector3d right)
        {
            Vector3d result = Vector3d.Create((left.X + right.X) / 2, (left.Y + right.Y) / 2, (left.Z + right.Z) / 2);
            result.Normalize();

            result.Multiply(left.Length());
            return result;
        }
        // Summary:
        //     Retrieves an empty 3-D vector.
        public static Vector3d Empty
        {
            get
            {
                return Vector3d.Create(0, 0, 0);
            }
        }



        public static Vector3d Zero = new Vector3d();

        // rounds to factor
        public void Round()
        {
            X = (double)((int)(X * 65536)) / 65536.0;
            Y = (double)((int)(Y * 65536)) / 65536.0;
            Z = (double)((int)(Z * 65536)) / 65536.0;
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
        public static Vector3d AddVectors(Vector3d left, Vector3d right)
        {
            return Vector3d.Create(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
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
            return Vector3d.Create(
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
        //public override bool Equals(object compare)
        //{
        //    Vector3d comp = (Vector3d)compare;
        //    return this.X == comp.X && this.Y == comp.Y && this.Z == comp.Z;
        //}
        //
        // Summary:
        //     Returns the hash code for the current instance.
        //
        // Returns:
        //     Hash code for the instance.
        //public override int GetHashCode()
        //{
        //    return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        //}
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
            return Math.Sqrt(X * X + Y * Y + Z * Z);
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
        public static double GetLength(Vector3d source)
        {
            return Math.Sqrt(source.X * source.X + source.Y * source.Y + source.Z * source.Z);

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
        public static double GetLengthSq(Vector3d source)
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
            return Vector3d.Create(
                left.X * (1.0 - interpolater) + right.X * interpolater,
                left.Y * (1.0 - interpolater) + right.Y * interpolater,
                left.Z * (1.0 - interpolater) + right.Z * interpolater);

        }


        public static Vector3d Midpoint(Vector3d left, Vector3d right)
        {
            Vector3d tmp = Vector3d.Create(
                left.X * (.5) + right.X * .5,
                left.Y * (.5) + right.Y * .5,
                left.Z * (.5) + right.Z * .5);
            tmp.Normalize();
            return tmp;
        }


        public static Vector3d Slerp(Vector3d left, Vector3d right, double interpolater)
        {
            double dot = Dot(left, right);
            while (dot < .98)
            {
                Vector3d middle = Midpoint(left, right);
                if (interpolater > .5)
                {
                    left = middle;
                    interpolater -= .5;
                    interpolater *= 2;
                }
                else
                {
                    right = middle;
                    interpolater *= 2;
                }
                dot = Dot(left, right);
            }

            Vector3d tmp = Lerp(left, right, interpolater);
            tmp.Normalize();
            return tmp;
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
        public static Vector3d MultiplyScalar(Vector3d source, double f)
        {
            Vector3d result = source.Copy();
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
            if (length != 0)
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

        public void RotateX(double radians)
        {
            double zTemp;
            double yTemp;
            //radians = -radians;
            yTemp = Y * Math.Cos(radians) - Z * Math.Sin(radians);
            zTemp = Y * Math.Sin(radians) + Z * Math.Cos(radians);
            Z = zTemp;
            Y = yTemp;
        }

        public void RotateZ(double radians)
        {
            double xTemp;
            double yTemp;
            //radians = -radians;
            xTemp = X * Math.Cos(radians) - Y * Math.Sin(radians);
            yTemp = X * Math.Sin(radians) + Y * Math.Cos(radians);
            Y = yTemp;
            X = xTemp;
        }

        public void RotateY(double radians)
        {
            double zTemp;
            double xTemp;
            //radians = -radians;
            zTemp = Z * Math.Cos(radians) - X * Math.Sin(radians);
            xTemp = Z * Math.Sin(radians) + X * Math.Cos(radians);
            X = xTemp;
            Z = zTemp;
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
        public static Vector3d SubtractVectors(Vector3d left, Vector3d right)
        {
            Vector3d result = left.Copy();
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

        public static Vector3d Parse(string data)
        {
            Vector3d newVector = new Vector3d();

            string[] list = data.Split( ',' );
            if (list.Length == 3)
            {
                newVector.X = double.Parse(list[0]);
                newVector.Y = double.Parse(list[1]);
                newVector.Z = double.Parse(list[2]);
            }
            return newVector;
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
            return Vector2d.Create((((ascention + Math.PI) % (2.0 * Math.PI))), ((declination + (Math.PI / 2.0))));

        }
        public Vector2d ToRaDec()
        {
            Vector2d point = ToSpherical();
            point.X = point.X / Math.PI * 12;
            point.Y = (point.Y / Math.PI * 180) - 90;

            //if (point.X == double.NaN || point.Y == double.NaN)
            //{
            //    point.X = point.Y = 0;
            //}
            return point;
        }

       


        public double DistanceToLine(Vector3d x1, Vector3d x2)
        {
            Vector3d t1 = Vector3d.SubtractVectors(x2, x1);
            Vector3d t2 = Vector3d.SubtractVectors(x1, this);
            Vector3d t3 = Vector3d.Cross(t1, t2);
            double d1 = t3.Length();
            Vector3d t4 = Vector3d.SubtractVectors(x2, x1);
            double d2 = t4.Length();
            return d1 / d2;

        }


        internal void TransformByMatrics(Matrix3d lookAtAdjust)
        {
            Vector3d temp = lookAtAdjust.Transform(this);
            this.X = temp.X;
            this.Y = temp.Y;
            this.Z = temp.Z;
        }

        internal static Vector3d TransformCoordinate(Vector3d vector3d, Matrix3d mat)
        {
            return mat.Transform(vector3d);
        }

    }
    public class Vector2d
    {
        public double X;
        public double Y;
        public Vector2d()
        {
        }
        
        public static Vector2d Lerp(Vector2d left, Vector2d right, double interpolater)
        {
            //if (Math.Abs(left.X - right.X) > 12)
            //{
            //    if (left.X > right.X)
            //    {
            //        right.X += 24;
            //    }
            //    else
            //    {
            //        left.X += 24;
            //    }
            //}
            return Vector2d.Create(left.X * (1 - interpolater) + right.X * interpolater, left.Y * (1 - interpolater) + right.Y * interpolater);

        }

        static public Vector2d CartesianToSpherical2(Vector3d vector)
        {
            double rho = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            double longitude = Math.Atan2(vector.Z, vector.X);
            double latitude = Math.Asin(vector.Y / rho);

            return Vector2d.Create(longitude / Math.PI * 180.0, latitude / Math.PI * 180.0);

        }

        public double Distance3d(Vector2d pointB)
        {
            Vector3d pnt1 = Coordinates.GeoTo3dDouble(pointB.Y, pointB.X);
            Vector3d pnt2 = Coordinates.GeoTo3dDouble(this.Y, this.X);

            Vector3d pntDiff = Vector3d.SubtractVectors(pnt1, pnt2);

            return pntDiff.Length() / Math.PI * 180;
        }

        public static Vector2d Average3d(Vector2d left, Vector2d right)
        {
            Vector3d pntLeft = Coordinates.GeoTo3dDouble(left.Y, left.X);
            Vector3d pntRight = Coordinates.GeoTo3dDouble(right.Y, right.X);

            Vector3d pntOut = Vector3d.AddVectors(pntLeft, pntRight);
            pntOut.Multiply(.5);
            pntOut.Normalize();

            return CartesianToSpherical2(pntOut);

        }

        public double Length
        {
            get
            {
                return (Math.Sqrt(X * X + Y * Y));
            }
        }

        //public static Vector2d Subtract(Vector2d vec)
        //{
        //    return Vector2d.Create(-vec.X, -vec.Y);

        //}

        public static Vector2d Create(double x, double y)
        {
            Vector2d temp = new Vector2d();

            temp.X = x;
            temp.Y = y;
            return temp;
        }

        public static Vector2d Subtract(Vector2d left, Vector2d right)
        {
            return Vector2d.Create(left.X - right.X, left.Y - right.Y);
        }
        public void Normalize()
        {
            double length = this.Length;
            if (length != 0)
            {
                X /= length;
                Y /= length;
            }
        }

        public void Extend(double factor)
        {
            X = X * factor;
            Y = Y * factor;
        }

        //internal static Vector2d Subtract(Vector2d v1, Vector2d v2)
        //{
        //    return Vector2d.Create(v1.X - v2.X, v1.Y - v2.Y);
        //}
    }

    public class Matrix3d 
    {
        private double _m11;
        private double _m12;
        private double _m13;
        private double _m14;
        private double _m21;
        private double _m22;
        private double _m23;
        private double _m24;
        private double _m31;
        private double _m32;
        private double _m33;
        private double _m34;
        private double _offsetX;
        private double _offsetY;
        private double _offsetZ;
        private double _m44;
        private bool _isNotKnownToBeIdentity;

        private static readonly Matrix3d s_identity;

        public Matrix3d()
        {
        }

        public static Matrix3d Create(double m11, double m12, double m13, double m14, double m21, double m22, double m23, double m24, double m31, double m32, double m33, double m34, double offsetX, double offsetY, double offsetZ, double m44)
        {
            Matrix3d temp = new Matrix3d();
            temp._m11 = m11;
            temp._m12 = m12;
            temp._m13 = m13;
            temp._m14 = m14;
            temp._m21 = m21;
            temp._m22 = m22;
            temp._m23 = m23;
            temp._m24 = m24;
            temp._m31 = m31;
            temp._m32 = m32;
            temp._m33 = m33;
            temp._m34 = m34;
            temp._offsetX = offsetX;
            temp._offsetY = offsetY;
            temp._offsetZ = offsetZ;
            temp._m44 = m44;
            temp._isNotKnownToBeIdentity = true;

            return temp;
        }

        public Matrix3d Clone()
        {
            Matrix3d tmp = new Matrix3d();
            tmp.Set(this);
            return tmp;
        }

       

        public static Matrix3d Identity
        {
            get
            {
                Matrix3d temp = new Matrix3d();
                temp.Set(s_identity);
                return temp;
            }
        }

        public void SetIdentity()
        {
            this.Set(s_identity);
        }

        public void Set(Matrix3d mat)
        {
            this._m11 = mat._m11;
            this._m12 = mat._m12;
            this._m13 = mat._m13;
            this._m14 = mat._m14;
            this._m21 = mat._m21;
            this._m22 = mat._m22;
            this._m23 = mat._m23;
            this._m24 = mat._m24;
            this._m31 = mat._m31;
            this._m32 = mat._m32;
            this._m33 = mat._m33;
            this._m34 = mat._m34;
            this._offsetX = mat._offsetX;
            this._offsetY = mat._offsetY;
            this._offsetZ = mat._offsetZ;
            this._m44 = mat._m44;
            this._isNotKnownToBeIdentity = true;
        }

        public float[] FloatArray()
        {
            float[] array = new float[16];

            array[0] = (float)_m11;
            array[1] = (float)_m12;
            array[2] = (float)_m13;
            array[3] = (float)_m14;
            array[4] = (float)_m21;
            array[5] = (float)_m22;
            array[6] = (float)_m23;
            array[7] = (float)_m24;
            array[8] = (float)_m31;
            array[9] = (float)_m32;
            array[10] = (float)_m33;
            array[11] = (float)_m34;
            array[12] = (float)_offsetX;
            array[13] = (float)_offsetY;
            array[14] = (float)_offsetZ;
            array[15] = (float)_m44;

            return array;

        }

        public bool IsIdentity
        {
            get
            {
                if (this.IsDistinguishedIdentity)
                {
                    return true;
                }
                if (((((this._m11 == 1) && (this._m12 == 0)) && ((this._m13 == 0) && (this._m14 == 0))) && (((this._m21 == 0) && (this._m22 == 1)) && ((this._m23 == 0) && (this._m24 == 0)))) && ((((this._m31 == 0) && (this._m32 == 0)) && ((this._m33 == 1) && (this._m34 == 0))) && (((this._offsetX == 0) && (this._offsetY == 0)) && ((this._offsetZ == 0) && (this._m44 == 1)))))
                {
                    this.IsDistinguishedIdentity = true;
                    return true;
                }
                return false;
            }
        }

        public void Prepend(Matrix3d matrix)
        {
            Set(MultiplyMatrix(matrix,this));
        }

        public void Append(Matrix3d matrix)
        {
            this.Multiply(matrix);
        }

        //public void Rotate(Quaternion quaternion)
        //{
        //    Vector3d center = new Vector3d();
        //    this *= CreateRotationMatrix(ref quaternion, ref center);
        //}

        //public void RotatePrepend(Quaternion quaternion)
        //{
        //    Vector3d center = new Vector3d();
        //    this = CreateRotationMatrix(ref quaternion, ref center) * this;
        //}

        //public void RotateAt(Quaternion quaternion, Vector3d center)
        //{
        //    this *= CreateRotationMatrix(ref quaternion, ref center);
        //}

        //public void RotateAtPrepend(Quaternion quaternion, Vector3d center)
        //{
        //    this = CreateRotationMatrix(ref quaternion, ref center) * this;
        //}

        public void Scale(Vector3d scale)
        {
            if (this.IsDistinguishedIdentity)
            {
                this.SetScaleMatrix( scale);
            }
            else
            {
                this._m11 *= scale.X;
                this._m12 *= scale.Y;
                this._m13 *= scale.Z;
                this._m21 *= scale.X;
                this._m22 *= scale.Y;
                this._m23 *= scale.Z;
                this._m31 *= scale.X;
                this._m32 *= scale.Y;
                this._m33 *= scale.Z;
                this._offsetX *= scale.X;
                this._offsetY *= scale.Y;
                this._offsetZ *= scale.Z;
            }
        }

        public void ScalePrepend(Vector3d scale)
        {
            if (this.IsDistinguishedIdentity)
            {
                this.SetScaleMatrix( scale);
            }
            else
            {
                this._m11 *= scale.X;
                this._m12 *= scale.X;
                this._m13 *= scale.X;
                this._m14 *= scale.X;
                this._m21 *= scale.Y;
                this._m22 *= scale.Y;
                this._m23 *= scale.Y;
                this._m24 *= scale.Y;
                this._m31 *= scale.Z;
                this._m32 *= scale.Z;
                this._m33 *= scale.Z;
                this._m34 *= scale.Z;
            }
        }

        public void ScaleAt(Vector3d scale, Vector3d center)
        {
            if (this.IsDistinguishedIdentity)
            {
                this.SetScaleMatrixCenter( scale,  center);
            }
            else
            {
                double num = this._m14 * center.X;
                this._m11 = num + (scale.X * (this._m11 - num));
                num = this._m14 * center.Y;
                this._m12 = num + (scale.Y * (this._m12 - num));
                num = this._m14 * center.Z;
                this._m13 = num + (scale.Z * (this._m13 - num));
                num = this._m24 * center.X;
                this._m21 = num + (scale.X * (this._m21 - num));
                num = this._m24 * center.Y;
                this._m22 = num + (scale.Y * (this._m22 - num));
                num = this._m24 * center.Z;
                this._m23 = num + (scale.Z * (this._m23 - num));
                num = this._m34 * center.X;
                this._m31 = num + (scale.X * (this._m31 - num));
                num = this._m34 * center.Y;
                this._m32 = num + (scale.Y * (this._m32 - num));
                num = this._m34 * center.Z;
                this._m33 = num + (scale.Z * (this._m33 - num));
                num = this._m44 * center.X;
                this._offsetX = num + (scale.X * (this._offsetX - num));
                num = this._m44 * center.Y;
                this._offsetY = num + (scale.Y * (this._offsetY - num));
                num = this._m44 * center.Z;
                this._offsetZ = num + (scale.Z * (this._offsetZ - num));
            }
        }

        public void ScaleAtPrepend(Vector3d scale, Vector3d center)
        {
            if (this.IsDistinguishedIdentity)
            {
                this.SetScaleMatrixCenter( scale,  center);
            }
            else
            {
                double num3 = center.X - (center.X * scale.X);
                double num2 = center.Y - (center.Y * scale.Y);
                double num = center.Z - (center.Z * scale.Z);
                this._offsetX += ((this._m11 * num3) + (this._m21 * num2)) + (this._m31 * num);
                this._offsetY += ((this._m12 * num3) + (this._m22 * num2)) + (this._m32 * num);
                this._offsetZ += ((this._m13 * num3) + (this._m23 * num2)) + (this._m33 * num);
                this._m44 += ((this._m14 * num3) + (this._m24 * num2)) + (this._m34 * num);
                this._m11 *= scale.X;
                this._m12 *= scale.X;
                this._m13 *= scale.X;
                this._m14 *= scale.X;
                this._m21 *= scale.Y;
                this._m22 *= scale.Y;
                this._m23 *= scale.Y;
                this._m24 *= scale.Y;
                this._m31 *= scale.Z;
                this._m32 *= scale.Z;
                this._m33 *= scale.Z;
                this._m34 *= scale.Z;
            }
        }

        public void Translate(Vector3d offset)
        {
            if (this.IsDistinguishedIdentity)
            {
                this.SetTranslationMatrix( offset);
            }
            else
            {
                this._m11 += this._m14 * offset.X;
                this._m12 += this._m14 * offset.Y;
                this._m13 += this._m14 * offset.Z;
                this._m21 += this._m24 * offset.X;
                this._m22 += this._m24 * offset.Y;
                this._m23 += this._m24 * offset.Z;
                this._m31 += this._m34 * offset.X;
                this._m32 += this._m34 * offset.Y;
                this._m33 += this._m34 * offset.Z;
                this._offsetX += this._m44 * offset.X;
                this._offsetY += this._m44 * offset.Y;
                this._offsetZ += this._m44 * offset.Z;
            }
        }

        public void TranslatePrepend(Vector3d offset)
        {
            if (this.IsDistinguishedIdentity)
            {
                this.SetTranslationMatrix( offset);
            }
            else
            {
                this._offsetX += ((this._m11 * offset.X) + (this._m21 * offset.Y)) + (this._m31 * offset.Z);
                this._offsetY += ((this._m12 * offset.X) + (this._m22 * offset.Y)) + (this._m32 * offset.Z);
                this._offsetZ += ((this._m13 * offset.X) + (this._m23 * offset.Y)) + (this._m33 * offset.Z);
                this._m44 += ((this._m14 * offset.X) + (this._m24 * offset.Y)) + (this._m34 * offset.Z);
            }
        }

        public static Matrix3d MultiplyMatrix(Matrix3d matrix1, Matrix3d matrix2)
        {
            if (matrix1.IsDistinguishedIdentity)
            {
                return matrix2;
            }
            if (matrix2.IsDistinguishedIdentity)
            {
                return matrix1;
            }
            return Matrix3d.Create((((matrix1._m11 * matrix2._m11) + (matrix1._m12 * matrix2._m21)) + (matrix1._m13 * matrix2._m31)) + (matrix1._m14 * matrix2._offsetX), (((matrix1._m11 * matrix2._m12) + (matrix1._m12 * matrix2._m22)) + (matrix1._m13 * matrix2._m32)) + (matrix1._m14 * matrix2._offsetY), (((matrix1._m11 * matrix2._m13) + (matrix1._m12 * matrix2._m23)) + (matrix1._m13 * matrix2._m33)) + (matrix1._m14 * matrix2._offsetZ), (((matrix1._m11 * matrix2._m14) + (matrix1._m12 * matrix2._m24)) + (matrix1._m13 * matrix2._m34)) + (matrix1._m14 * matrix2._m44), (((matrix1._m21 * matrix2._m11) + (matrix1._m22 * matrix2._m21)) + (matrix1._m23 * matrix2._m31)) + (matrix1._m24 * matrix2._offsetX), (((matrix1._m21 * matrix2._m12) + (matrix1._m22 * matrix2._m22)) + (matrix1._m23 * matrix2._m32)) + (matrix1._m24 * matrix2._offsetY), (((matrix1._m21 * matrix2._m13) + (matrix1._m22 * matrix2._m23)) + (matrix1._m23 * matrix2._m33)) + (matrix1._m24 * matrix2._offsetZ), (((matrix1._m21 * matrix2._m14) + (matrix1._m22 * matrix2._m24)) + (matrix1._m23 * matrix2._m34)) + (matrix1._m24 * matrix2._m44), (((matrix1._m31 * matrix2._m11) + (matrix1._m32 * matrix2._m21)) + (matrix1._m33 * matrix2._m31)) + (matrix1._m34 * matrix2._offsetX), (((matrix1._m31 * matrix2._m12) + (matrix1._m32 * matrix2._m22)) + (matrix1._m33 * matrix2._m32)) + (matrix1._m34 * matrix2._offsetY), (((matrix1._m31 * matrix2._m13) + (matrix1._m32 * matrix2._m23)) + (matrix1._m33 * matrix2._m33)) + (matrix1._m34 * matrix2._offsetZ), (((matrix1._m31 * matrix2._m14) + (matrix1._m32 * matrix2._m24)) + (matrix1._m33 * matrix2._m34)) + (matrix1._m34 * matrix2._m44), (((matrix1._offsetX * matrix2._m11) + (matrix1._offsetY * matrix2._m21)) + (matrix1._offsetZ * matrix2._m31)) + (matrix1._m44 * matrix2._offsetX), (((matrix1._offsetX * matrix2._m12) + (matrix1._offsetY * matrix2._m22)) + (matrix1._offsetZ * matrix2._m32)) + (matrix1._m44 * matrix2._offsetY), (((matrix1._offsetX * matrix2._m13) + (matrix1._offsetY * matrix2._m23)) + (matrix1._offsetZ * matrix2._m33)) + (matrix1._m44 * matrix2._offsetZ), (((matrix1._offsetX * matrix2._m14) + (matrix1._offsetY * matrix2._m24)) + (matrix1._offsetZ * matrix2._m34)) + (matrix1._m44 * matrix2._m44));
        }


        public Vector3d Transform(Vector3d point)
        {
            Vector3d temp = new Vector3d();
            if (!this.IsDistinguishedIdentity)
            {
                double x = point.X;
                double y = point.Y;
                double z = point.Z;
                temp.X = (((x * this._m11) + (y * this._m21)) + (z * this._m31)) + this._offsetX;
                temp.Y = (((x * this._m12) + (y * this._m22)) + (z * this._m32)) + this._offsetY;
                temp.Z = (((x * this._m13) + (y * this._m23)) + (z * this._m33)) + this._offsetZ;
                if (!this.IsAffine)
                {
                    double num4 = (((x * this._m14) + (y * this._m24)) + (z * this._m34)) + this._m44;
                    temp.X /= num4;
                    temp.Y /= num4;
                    temp.Z /= num4;
                }
            }

            return temp;
        }

        internal void TransformTo(Vector3d input, Vector3d output)
        {
            output.X = (((input.X * this._m11) + (input.Y * this._m21)) + (input.Z * this._m31)) + this._offsetX;
            output.Y = (((input.X * this._m12) + (input.Y * this._m22)) + (input.Z * this._m32)) + this._offsetY;
            output.Z = (((input.X * this._m13) + (input.Y * this._m23)) + (input.Z * this._m33)) + this._offsetZ;

            double num4 = (((input.X * this._m14) + (input.Y * this._m24)) + (input.Z * this._m34)) + this._m44;
            output.X /= num4;
            output.Y /= num4;
            output.Z /= num4;
        }
        //internal void TransformTo(Vector3d input, Vector3d output)
        //{
        //    if (!this.IsDistinguishedIdentity)
        //    {
        //        double x = input.X;
        //        double y = input.Y;
        //        double z = input.Z;
        //        output.X = (((x * this._m11) + (y * this._m21)) + (z * this._m31)) + this._offsetX;
        //        output.Y = (((x * this._m12) + (y * this._m22)) + (z * this._m32)) + this._offsetY;
        //        output.Z = (((x * this._m13) + (y * this._m23)) + (z * this._m33)) + this._offsetZ;
        //        if (!this.IsAffine)
        //        {
        //            double num4 = (((x * this._m14) + (y * this._m24)) + (z * this._m34)) + this._m44;
        //            output.X /= num4;
        //            output.Y /= num4;
        //            output.Z /= num4;
        //        }
        //    }
        //}
        public void TransformArray(Vector3d[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    this.MultiplyPoint( points[i]);
                }
            }
        }

        public void ProjectArrayToScreen(Vector3d[] input, Vector3d[] output)
        {
            if (input != null && output != null)
            {
                bool affine = this.IsAffine;
                for (int i = 0; i < input.Length; i++)
                {
                    double x = input[i].X;
                    double y = input[i].Y;
                    double z = input[i].Z;
                    if (affine)
                    {
                        output[i].X = (((((x * this._m11) + (y * this._m21)) + (z * this._m31)) + this._offsetX) );
                        output[i].Y = (((((x * this._m12) + (y * this._m22)) + (z * this._m32)) + this._offsetY) );
                        output[i].Z = (((x * this._m13) + (y * this._m23)) + (z * this._m33)) + this._offsetZ;
                    }
                    else
                    {
                        double num4 = (((x * this._m14) + (y * this._m24)) + (z * this._m34)) + this._m44;
                        output[i].X = ((((((x * this._m11) + (y * this._m21)) + (z * this._m31)) + this._offsetX) / num4) )  ;
                        output[i].Y = ((((((x * this._m12) + (y * this._m22)) + (z * this._m32)) + this._offsetY) / num4) )  ;
                        output[i].Z = ((((x * this._m13) + (y * this._m23)) + (z * this._m33)) + this._offsetZ) / num4;
                    }
                }
            }
        }

        public Vector3d ProjectToScreen(Vector3d input, double width, double height)
        {
            Vector3d output = new Vector3d();
            double x = input.X;
            double y = input.Y;
            double z = input.Z;
            if (this.IsAffine)
            {
                output.X = (((((x * this._m11) + (y * this._m21)) + (z * this._m31)) + this._offsetX) + .5) * width;
                output.Y = (-((((x * this._m12) + (y * this._m22)) + (z * this._m32)) + this._offsetY) + .5) * height;
                output.Z = (((x * this._m13) + (y * this._m23)) + (z * this._m33)) + this._offsetZ;
            }
            else
            {
                double num4 = (((x * this._m14) + (y * this._m24)) + (z * this._m34)) + this._m44;
                output.X = ((((((x * this._m11) + (y * this._m21)) + (z * this._m31)) + this._offsetX) / num4) + .5) * width; ;
                output.Y = (-(((((x * this._m12) + (y * this._m22)) + (z * this._m32)) + this._offsetY) / num4) + .5) * height; ;
                output.Z = ((((x * this._m13) + (y * this._m23)) + (z * this._m33)) + this._offsetZ) / num4;
            }
            return output;
        }
    


        //public Vector3d Transform(Vector3d vector)
        //{
        //    this.MultiplyVector(ref vector);
        //    return vector;
        //}

        //public void Transform(Vector3d[] vectors)
        //{
        //    if (vectors != null)
        //    {
        //        for (int i = 0; i < vectors.Length; i++)
        //        {
        //            this.MultiplyVector(ref vectors[i]);
        //        }
        //    }
        //}

        public bool IsAffine
        {
            get
            {
                if (this.IsDistinguishedIdentity)
                {
                    return true;
                }
                if (((this._m14 == 0) && (this._m24 == 0)) && (this._m34 == 0))
                {
                    return (this._m44 == 1);
                }
                return false;
            }
        }

        public double Determinant
        {
            get
            {
                if (this.IsDistinguishedIdentity)
                {
                    return 1;
                }
                if (this.IsAffine)
                {
                    return this.GetNormalizedAffineDeterminant();
                }
                double num6 = (this._m13 * this._m24) - (this._m23 * this._m14);
                double num5 = (this._m13 * this._m34) - (this._m33 * this._m14);
                double num4 = (this._m13 * this._m44) - (this._offsetZ * this._m14);
                double num3 = (this._m23 * this._m34) - (this._m33 * this._m24);
                double num2 = (this._m23 * this._m44) - (this._offsetZ * this._m24);
                double num = (this._m33 * this._m44) - (this._offsetZ * this._m34);
                double num10 = ((this._m22 * num5) - (this._m32 * num6)) - (this._m12 * num3);
                double num9 = ((this._m12 * num2) - (this._m22 * num4)) + (this._offsetY * num6);
                double num8 = ((this._m32 * num4) - (this._offsetY * num5)) - (this._m12 * num);
                double num7 = ((this._m22 * num) - (this._m32 * num2)) + (this._offsetY * num3);
                return ((((this._offsetX * num10) + (this._m31 * num9)) + (this._m21 * num8)) + (this._m11 * num7));
            }
        }

        public bool HasInverse
        {
            get
            {
                return !DoubleUtilities.IsZero(this.Determinant);
            }
        }
        public void Invert()
        {
            if (!this.InvertCore())
            {
                return;

            }
        }

        public void Transpose()
        {
            Matrix3d that = new Matrix3d();
            that.Set(this);

            this._m12 = that._m21;
            this._m13 = that._m31;
            this._m14 = that._offsetX;
            this._m23 = that._m32;
            this._m24 = that._offsetY;
            this._m34 = that._offsetZ;
            this._m21 = that._m12;
            this._m31 = that._m13;
            this._offsetX = that._m14;
            this._m32 = that._m23;
            this._offsetY = that._m24;
            this._offsetZ = that._m34;

        }

        //private static void Swap(ref double a, ref double b)
        //{
        //    double temp = a;
        //    a = b;
        //    b = temp;
        //}

        public double M11
        {
            get
            {
                if (this.IsDistinguishedIdentity)
                {
                    return 1;
                }
                return this._m11;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m11 = value;
            }
        }

        public double M12
        {
            get
            {
                return this._m12;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m12 = value;
            }
        }

        public double M13
        {
            get
            {
                return this._m13;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m13 = value;
            }
        }

        public double M14
        {
            get
            {
                return this._m14;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m14 = value;
            }
        }

        public double M21
        {
            get
            {
                return this._m21;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m21 = value;
            }
        }
        public double M22
        {
            get
            {
                if (this.IsDistinguishedIdentity)
                {
                    return 1;
                }
                return this._m22;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m22 = value;
            }
        }
        public double M23
        {
            get
            {
                return this._m23;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m23 = value;
            }
        }
        public double M24
        {
            get
            {
                return this._m24;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m24 = value;
            }
        }
        public double M31
        {
            get
            {
                return this._m31;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m31 = value;
            }
        }
        public double M32
        {
            get
            {
                return this._m32;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m32 = value;
            }
        }
        public double M33
        {
            get
            {
                if (this.IsDistinguishedIdentity)
                {
                    return 1;
                }
                return this._m33;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m33 = value;
            }
        }
        public double M34
        {
            get
            {
                return this._m34;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m34 = value;
            }
        }

        public double M41
        {
            get
            {
                return OffsetX;
            }
            set
            {
                OffsetX = value;
            }
        }

        public double M42
        {
            get
            {
                return OffsetY;
            }
            set
            {
                OffsetY = value;
            }
        }

        public double M43
        {
            get
            {
                return OffsetZ;
            }
            set
            {
                OffsetZ = value;
            }
        }

        public double OffsetX
        {
            get
            {
                return this._offsetX;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._offsetX = value;
            }
        }
        public double OffsetY
        {
            get
            {
                return this._offsetY;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._offsetY = value;
            }
        }
        public double OffsetZ
        {
            get
            {
                return this._offsetZ;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._offsetZ = value;
            }
        }
        public double M44
        {
            get
            {
                if (this.IsDistinguishedIdentity)
                {
                    return 1;
                }
                return this._m44;
            }
            set
            {
                if (this.IsDistinguishedIdentity)
                {
                    Set(s_identity);
                    this.IsDistinguishedIdentity = false;
                }
                this._m44 = value;
            }
        }
        private void SetScaleMatrix( Vector3d scale)
        {
            this._m11 = scale.X;
            this._m22 = scale.Y;
            this._m33 = scale.Z;
            this._m44 = 1;
            this.IsDistinguishedIdentity = false;
        }

        private void SetScaleMatrixCenter( Vector3d scale,  Vector3d center)
        {
            this._m11 = scale.X;
            this._m22 = scale.Y;
            this._m33 = scale.Z;
            this._m44 = 1;
            this._offsetX = center.X - (center.X * scale.X);
            this._offsetY = center.Y - (center.Y * scale.Y);
            this._offsetZ = center.Z - (center.Z * scale.Z);
            this.IsDistinguishedIdentity = false;
        }

        private void SetTranslationMatrix( Vector3d offset)
        {
            this._m11 = this._m22 = this._m33 = this._m44 = 1;
            this._offsetX = offset.X;
            this._offsetY = offset.Y;
            this._offsetZ = offset.Z;
            this.IsDistinguishedIdentity = false;
        }

        //internal static Matrix3d CreateRotationMatrix(ref Quaternion quaternion, ref Vector3d center)
        //{
        //    Matrix3d matrixd = s_identity;
        //    matrixd.IsDistinguishedIdentity = false;
        //    double num12 = quaternion.X + quaternion.X;
        //    double num2 = quaternion.Y + quaternion.Y;
        //    double num = quaternion.Z + quaternion.Z;
        //    double num11 = quaternion.X * num12;
        //    double num10 = quaternion.X * num2;
        //    double num9 = quaternion.X * num;
        //    double num8 = quaternion.Y * num2;
        //    double num7 = quaternion.Y * num;
        //    double num6 = quaternion.Z * num;
        //    double num5 = quaternion.W * num12;
        //    double num4 = quaternion.W * num2;
        //    double num3 = quaternion.W * num;
        //    matrixd._m11 = 1 - (num8 + num6);
        //    matrixd._m12 = num10 + num3;
        //    matrixd._m13 = num9 - num4;
        //    matrixd._m21 = num10 - num3;
        //    matrixd._m22 = 1 - (num11 + num6);
        //    matrixd._m23 = num7 + num5;
        //    matrixd._m31 = num9 + num4;
        //    matrixd._m32 = num7 - num5;
        //    matrixd._m33 = 1 - (num11 + num8);
        //    if (((center.X != 0) || (center.Y != 0)) || (center.Z != 0))
        //    {
        //        matrixd._offsetX = (((-center.X * matrixd._m11) - (center.Y * matrixd._m21)) - (center.Z * matrixd._m31)) + center.X;
        //        matrixd._offsetY = (((-center.X * matrixd._m12) - (center.Y * matrixd._m22)) - (center.Z * matrixd._m32)) + center.Y;
        //        matrixd._offsetZ = (((-center.X * matrixd._m13) - (center.Y * matrixd._m23)) - (center.Z * matrixd._m33)) + center.Z;
        //    }
        //    return matrixd;
        //}

        private void MultiplyPoint( Vector3d point)
        {
            if (!this.IsDistinguishedIdentity)
            {
                double x = point.X;
                double y = point.Y;
                double z = point.Z;
                point.X = (((x * this._m11) + (y * this._m21)) + (z * this._m31)) + this._offsetX;
                point.Y = (((x * this._m12) + (y * this._m22)) + (z * this._m32)) + this._offsetY;
                point.Z = (((x * this._m13) + (y * this._m23)) + (z * this._m33)) + this._offsetZ;
                if (!this.IsAffine)
                {
                    double num4 = (((x * this._m14) + (y * this._m24)) + (z * this._m34)) + this._m44;
                    point.X /= num4;
                    point.Y /= num4;
                    point.Z /= num4;
                }
            }
        }

        public void MultiplyVector( Vector3d vector)
        {
            if (!this.IsDistinguishedIdentity)
            {
                double x = vector.X;
                double y = vector.Y;
                double z = vector.Z;
                vector.X = ((x * this._m11) + (y * this._m21)) + (z * this._m31);
                vector.Y = ((x * this._m12) + (y * this._m22)) + (z * this._m32);
                vector.Z = ((x * this._m13) + (y * this._m23)) + (z * this._m33);
            }
        }

        private double GetNormalizedAffineDeterminant()
        {
            double num3 = (this._m12 * this._m23) - (this._m22 * this._m13);
            double num2 = (this._m32 * this._m13) - (this._m12 * this._m33);
            double num = (this._m22 * this._m33) - (this._m32 * this._m23);
            return (((this._m31 * num3) + (this._m21 * num2)) + (this._m11 * num));
        }

        private bool NormalizedAffineInvert()
        {
            double num11 = (this._m12 * this._m23) - (this._m22 * this._m13);
            double num10 = (this._m32 * this._m13) - (this._m12 * this._m33);
            double num9 = (this._m22 * this._m33) - (this._m32 * this._m23);
            double num8 = ((this._m31 * num11) + (this._m21 * num10)) + (this._m11 * num9);
            if (DoubleUtilities.IsZero(num8))
            {
                return false;
            }
            double num20 = (this._m21 * this._m13) - (this._m11 * this._m23);
            double num19 = (this._m11 * this._m33) - (this._m31 * this._m13);
            double num18 = (this._m31 * this._m23) - (this._m21 * this._m33);
            double num7 = (this._m11 * this._m22) - (this._m21 * this._m12);
            double num6 = (this._m11 * this._m32) - (this._m31 * this._m12);
            double num5 = (this._m11 * this._offsetY) - (this._offsetX * this._m12);
            double num4 = (this._m21 * this._m32) - (this._m31 * this._m22);
            double num3 = (this._m21 * this._offsetY) - (this._offsetX * this._m22);
            double num2 = (this._m31 * this._offsetY) - (this._offsetX * this._m32);
            double num17 = ((this._m23 * num5) - (this._offsetZ * num7)) - (this._m13 * num3);
            double num16 = ((this._m13 * num2) - (this._m33 * num5)) + (this._offsetZ * num6);
            double num15 = ((this._m33 * num3) - (this._offsetZ * num4)) - (this._m23 * num2);
            double num14 = num7;
            double num13 = -num6;
            double num12 = num4;
            double num = 1 / num8;
            this._m11 = num9 * num;
            this._m12 = num10 * num;
            this._m13 = num11 * num;
            this._m21 = num18 * num;
            this._m22 = num19 * num;
            this._m23 = num20 * num;
            this._m31 = num12 * num;
            this._m32 = num13 * num;
            this._m33 = num14 * num;
            this._offsetX = num15 * num;
            this._offsetY = num16 * num;
            this._offsetZ = num17 * num;
            return true;
        }


        private bool InvertCore()
        {
            if (!this.IsDistinguishedIdentity)
            {
                if (this.IsAffine)
                {
                    return this.NormalizedAffineInvert();
                }
                double num7 = (this._m13 * this._m24) - (this._m23 * this._m14);
                double num6 = (this._m13 * this._m34) - (this._m33 * this._m14);
                double num5 = (this._m13 * this._m44) - (this._offsetZ * this._m14);
                double num4 = (this._m23 * this._m34) - (this._m33 * this._m24);
                double num3 = (this._m23 * this._m44) - (this._offsetZ * this._m24);
                double num2 = (this._m33 * this._m44) - (this._offsetZ * this._m34);
                double num12 = ((this._m22 * num6) - (this._m32 * num7)) - (this._m12 * num4);
                double num11 = ((this._m12 * num3) - (this._m22 * num5)) + (this._offsetY * num7);
                double num10 = ((this._m32 * num5) - (this._offsetY * num6)) - (this._m12 * num2);
                double num9 = ((this._m22 * num2) - (this._m32 * num3)) + (this._offsetY * num4);
                double num8 = (((this._offsetX * num12) + (this._m31 * num11)) + (this._m21 * num10)) + (this._m11 * num9);
                if (DoubleUtilities.IsZero(num8))
                {
                    return false;
                }
                double num24 = ((this._m11 * num4) - (this._m21 * num6)) + (this._m31 * num7);
                double num23 = ((this._m21 * num5) - (this._offsetX * num7)) - (this._m11 * num3);
                double num22 = ((this._m11 * num2) - (this._m31 * num5)) + (this._offsetX * num6);
                double num21 = ((this._m31 * num3) - (this._offsetX * num4)) - (this._m21 * num2);
                num7 = (this._m11 * this._m22) - (this._m21 * this._m12);
                num6 = (this._m11 * this._m32) - (this._m31 * this._m12);
                num5 = (this._m11 * this._offsetY) - (this._offsetX * this._m12);
                num4 = (this._m21 * this._m32) - (this._m31 * this._m22);
                num3 = (this._m21 * this._offsetY) - (this._offsetX * this._m22);
                num2 = (this._m31 * this._offsetY) - (this._offsetX * this._m32);
                double num20 = ((this._m13 * num4) - (this._m23 * num6)) + (this._m33 * num7);
                double num19 = ((this._m23 * num5) - (this._offsetZ * num7)) - (this._m13 * num3);
                double num18 = ((this._m13 * num2) - (this._m33 * num5)) + (this._offsetZ * num6);
                double num17 = ((this._m33 * num3) - (this._offsetZ * num4)) - (this._m23 * num2);
                double num16 = ((this._m24 * num6) - (this._m34 * num7)) - (this._m14 * num4);
                double num15 = ((this._m14 * num3) - (this._m24 * num5)) + (this._m44 * num7);
                double num14 = ((this._m34 * num5) - (this._m44 * num6)) - (this._m14 * num2);
                double num13 = ((this._m24 * num2) - (this._m34 * num3)) + (this._m44 * num4);
                double num = 1 / num8;
                this._m11 = num9 * num;
                this._m12 = num10 * num;
                this._m13 = num11 * num;
                this._m14 = num12 * num;
                this._m21 = num21 * num;
                this._m22 = num22 * num;
                this._m23 = num23 * num;
                this._m24 = num24 * num;
                this._m31 = num13 * num;
                this._m32 = num14 * num;
                this._m33 = num15 * num;
                this._m34 = num16 * num;
                this._offsetX = num17 * num;
                this._offsetY = num18 * num;
                this._offsetZ = num19 * num;
                this._m44 = num20 * num;
            }
            return true;
        }

        public static Matrix3d LookAtLH(Vector3d cameraPosition, Vector3d cameraTarget, Vector3d cameraUpVector)
        {

            Vector3d zaxis = Vector3d.SubtractVectors(cameraTarget, cameraPosition);
            zaxis.Normalize();
            Vector3d xaxis = Vector3d.Cross(cameraUpVector, zaxis);
            xaxis.Normalize();
            Vector3d yaxis = Vector3d.Cross(zaxis, xaxis);

            Matrix3d mat = Matrix3d.Create(xaxis.X, yaxis.X, zaxis.X, 0, xaxis.Y, yaxis.Y, zaxis.Y, 0, xaxis.Z, yaxis.Z, zaxis.Z, 0, -Vector3d.Dot(xaxis, cameraPosition), -Vector3d.Dot(yaxis, cameraPosition), -Vector3d.Dot(zaxis, cameraPosition), 1);
            return mat;
        }

        private static Matrix3d CreateIdentity()
        {
            Matrix3d matrixd = Matrix3d.Create(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            matrixd.IsDistinguishedIdentity = true;
            return matrixd;
        }

        private bool IsDistinguishedIdentity
        {
            get
            {
                return !this._isNotKnownToBeIdentity;
            }
            set
            {
                this._isNotKnownToBeIdentity = !value;
            }
        }
        //public static bool operator ==(Matrix3d matrix1, Matrix3d matrix2)
        //{
        //    if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
        //    {
        //        return (matrix1.IsIdentity == matrix2.IsIdentity);
        //    }
        //    if (((((matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12)) && ((matrix1.M13 == matrix2.M13) && (matrix1.M14 == matrix2.M14))) && (((matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22)) && ((matrix1.M23 == matrix2.M23) && (matrix1.M24 == matrix2.M24)))) && ((((matrix1.M31 == matrix2.M31) && (matrix1.M32 == matrix2.M32)) && ((matrix1.M33 == matrix2.M33) && (matrix1.M34 == matrix2.M34))) && (((matrix1.OffsetX == matrix2.OffsetX) && (matrix1.OffsetY == matrix2.OffsetY)) && (matrix1.OffsetZ == matrix2.OffsetZ))))
        //    {
        //        return (matrix1.M44 == matrix2.M44);
        //    }
        //    return false;
        //}

        //public static bool operator !=(Matrix3d matrix1, Matrix3d matrix2)
        //{
        //    return !(matrix1 == matrix2);
        //}

        public static bool Equals(Matrix3d matrix1, Matrix3d matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return (matrix1.IsIdentity == matrix2.IsIdentity);
            }
            if ((((matrix1.M11==(matrix2.M11) && matrix1.M12==(matrix2.M12)) && (matrix1.M13==(matrix2.M13) && matrix1.M14==(matrix2.M14))) && ((matrix1.M21==(matrix2.M21) && matrix1.M22==(matrix2.M22)) && (matrix1.M23==(matrix2.M23) && matrix1.M24==(matrix2.M24)))) && (((matrix1.M31==(matrix2.M31) && matrix1.M32==(matrix2.M32)) && (matrix1.M33==(matrix2.M33) && matrix1.M34==(matrix2.M34))) && ((matrix1.OffsetX==(matrix2.OffsetX) && matrix1.OffsetY==(matrix2.OffsetY)) && matrix1.OffsetZ==(matrix2.OffsetZ))))
            {
                return matrix1.M44==(matrix2.M44);
            }
            return false;
        }

        //public override bool Equals(object o)
        //{
        //    if ((o == null) || !(o is Matrix3d))
        //    {
        //        return false;
        //    }
        //    Matrix3d matrixd = (Matrix3d)o;
        //    return Equals(this, matrixd);
        //}

        //public bool Equals(Matrix3d value)
        //{
        //    return Equals(this, value);
        //}

        //public override int GetHashCode()
        //{
        //    if (this.IsDistinguishedIdentity)
        //    {
        //        return 0;
        //    }
        //    return (((((((((((((((this.M11.GetHashCode() ^ this.M12.GetHashCode()) ^ this.M13.GetHashCode()) ^ this.M14.GetHashCode()) ^ this.M21.GetHashCode()) ^ this.M22.GetHashCode()) ^ this.M23.GetHashCode()) ^ this.M24.GetHashCode()) ^ this.M31.GetHashCode()) ^ this.M32.GetHashCode()) ^ this.M33.GetHashCode()) ^ this.M34.GetHashCode()) ^ this.OffsetX.GetHashCode()) ^ this.OffsetY.GetHashCode()) ^ this.OffsetZ.GetHashCode()) ^ this.M44.GetHashCode());
        //}

        //public override string ToString()
        //{
        //    return this.ConvertToString(null, null);
        //}

        //string IFormattable.ToString(string format, IFormatProvider provider)
        //{
        //    return this.ConvertToString(format, provider);
        //}

        //private string ConvertToString(string format, IFormatProvider provider)
        //{
        //    if (this.IsIdentity)
        //    {
        //        return "Identity";
        //    }
        //    char numericListSeparator = ',';
        //    return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}{0}{7:" + format + "}{0}{8:" + format + "}{0}{9:" + format + "}{0}{10:" + format + "}{0}{11:" + format + "}{0}{12:" + format + "}{0}{13:" + format + "}{0}{14:" + format + "}{0}{15:" + format + "}{0}{16:" + format + "}", new object[] { 
        //    numericListSeparator, this._m11, this._m12, this._m13, this._m14, this._m21, this._m22, this._m23, this._m24, this._m31, this._m32, this._m33, this._m34, this._offsetX, this._offsetY, this._offsetZ, 
        //    this._m44
        // });
        //}

        public static Matrix3d FromMatrix2d(Matrix2d mat)
        {
            Matrix3d mat3d = Matrix3d.CreateIdentity();

            mat3d.M11 = mat.M11;
            mat3d.M12 = mat.M12;
            mat3d.M13 = mat.M13;
            mat3d.M21 = mat.M21;
            mat3d.M22 = mat.M22;
            mat3d.M23 = mat.M23;
            mat3d.M31 = mat.M31;
            mat3d.M32 = mat.M32;
            mat3d.M33 = mat.M33;
            mat3d._isNotKnownToBeIdentity = true;

            return mat3d;
        }


        static Matrix3d()
        {
            s_identity = CreateIdentity();
        }

        public static Matrix3d RotationYawPitchRoll(double heading, double pitch, double roll)
        {
            Matrix3d matX = RotationX(pitch);
            Matrix3d matY = RotationY(heading);
            Matrix3d matZ = RotationZ(roll);

            return Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(matY, matX), matZ);
        }

        internal static Matrix3d RotationY(double p)
        {
            double v = p;

            Matrix3d matNew = Matrix3d.Identity;
            matNew._m11 = Math.Cos(v);
            matNew._m22 = 1;
            matNew._m31 = Math.Sin(v);
            matNew._m13 = -Math.Sin(v);
            matNew._m33 = Math.Cos(v);
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }

        internal static Matrix3d RotationX(double p)
        {
            double v = p;

            Matrix3d matNew = Matrix3d.Identity;
            matNew._m11 = 1;
            matNew._m22 = Math.Cos(v);
            matNew._m32 = -Math.Sin(v);
            matNew._m23 = Math.Sin(v);
            matNew._m33 = Math.Cos(v);
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }
        internal static Matrix3d RotationZ(double p)
        {
            double v = p;

            Matrix3d matNew = Matrix3d.Identity;
            matNew._m11 = Math.Cos(v);
            matNew._m21 = -Math.Sin(v);
            matNew._m12 = Math.Sin(v);
            matNew._m22 = Math.Cos(v);
            matNew._m33 = 1;
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }
        internal static Matrix3d Scaling(double x, double y, double z)
        {
            Matrix3d matNew = Matrix3d.Identity;
            matNew._m11 = x;
            matNew._m22 = y;
            matNew._m33 = z;
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }

        internal static Matrix3d TranslationXYZ(double x, double y, double z)
        {
            Matrix3d matNew = Matrix3d.Identity;
            matNew.OffsetX = x;
            matNew.OffsetY = y;
            matNew.OffsetZ = z;
            matNew._isNotKnownToBeIdentity = true;
            return matNew;
        }

        internal void Multiply(Matrix3d mat)
        {
            Set(Matrix3d.MultiplyMatrix(this, mat));
        }

        public static Matrix3d PerspectiveFovLH(double fieldOfViewY, double aspectRatio, double znearPlane, double zfarPlane)
        {
            double h = 1 / Math.Tan(fieldOfViewY / 2);
            double w = h / aspectRatio;

            return Matrix3d.Create(w, 0, 0, 0, 0, h, 0, 0, 0, 0, zfarPlane / (zfarPlane - znearPlane), 1, 0, 0, -znearPlane * zfarPlane / (zfarPlane - znearPlane), 0);
        }

        public static Matrix3d PerspectiveOffCenterLH(double left, double right, double bottom, double top, double znearPlane, double zfarPlane)
        {
            return Matrix3d.Create(
                2 * znearPlane / (right - left), 0, 0, 0,
                0, 2 * znearPlane / (top - bottom), 0, 0,
                (left + right) / (left - right), (top + bottom) / (bottom - top), zfarPlane / (zfarPlane - znearPlane), 1,
                0, 0, znearPlane * zfarPlane / (znearPlane - zfarPlane), 0

                );
        }

        public static Matrix3d InvertMatrix(Matrix3d matrix3d)
        {
            Matrix3d mat = matrix3d.Clone();
            mat.Invert();
            return mat;
        }

        public static Matrix3d Translation(Vector3d vector3d)
        {
            return Matrix3d.TranslationXYZ(vector3d.X, vector3d.Y, vector3d.Z);
        }

        static public Matrix3d GetMapMatrix(Coordinates center, double fieldWidth, double fieldHeight, double rotation)
        {
            double offsetX = 0;
            double offsetY = 0;

            offsetX = -(((center.Lng + 180 - (fieldWidth / 2)) / 360));
            offsetY = -((1 - ((center.Lat + 90 + (fieldHeight / 2)) / 180)));

            Matrix2d mat = new Matrix2d();

            double scaleX = 0;
            double scaleY = 0;

            scaleX = 360 / fieldWidth;
            scaleY = 180 / fieldHeight;
            mat = Matrix2d.Multiply(mat, Matrix2d.Translation(offsetX, offsetY));
            mat = Matrix2d.Multiply(mat, Matrix2d.Scaling(scaleX, scaleY));
            if (rotation != 0)
            {
                mat = Matrix2d.Multiply(mat,Matrix2d.Translation(-.5, -.5));
                mat = Matrix2d.Multiply(mat,Matrix2d.Rotation(rotation));
                mat = Matrix2d.Multiply(mat,Matrix2d.Translation(.5, .5));
            }



            return Matrix3d.FromMatrix2d(mat);
        }

        
    }

    public class Matrix2d
    {
        public double M11 = 1;
        public double M12;
        public double M13;
        public double M21;
        public double M22 = 1;
        public double M23;
        public double M31;
        public double M32;
        public double M33 = 1;

        public Matrix2d()
        {
        }
        public static Matrix2d Create(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
        {
            Matrix2d mat = new Matrix2d();
            mat.M11 = m11;
            mat.M12 = m12;
            mat.M13 = m13;
            mat.M21 = m21;
            mat.M22 = m22;
            mat.M23 = m23;
            mat.M31 = m31;
            mat.M32 = m32;
            mat.M33 = m33;

            return mat;
        }

        public static Matrix2d Rotation(double angle)
        {
            Matrix2d mat = new Matrix2d();
            mat.M11 = Math.Cos(angle);
            mat.M21 = -Math.Sin(angle);
            mat.M12 = Math.Sin(angle);
            mat.M22 = Math.Cos(angle);
            return mat;
        }

        public static Matrix2d Translation(double x, double y)
        {
            Matrix2d mat = new Matrix2d();
            mat.M31 = x;
            mat.M32 = y;

            return mat;
        }

        public static Matrix2d Scaling(double x, double y)
        {
            Matrix2d mat = new Matrix2d();
            mat.M11 = x;
            mat.M22 = y;
            return mat;
        }

        public static Matrix2d Multiply(Matrix2d matrix1, Matrix2d matrix2)
        {

            return Matrix2d.Create
            (
                (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)),
                (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)),
                (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)),
                (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)),
                (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)),
                (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)),
                (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)),
                (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)),
                (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33))
            );
        }

        public static Matrix2d RotateAt(double angle, Vector2d pnt)
        {
            Matrix2d matT0 = Matrix2d.Translation(-pnt.X, -pnt.Y);
            Matrix2d matR = Matrix2d.Rotation(angle);
            Matrix2d matT1 = Matrix2d.Translation(pnt.X, pnt.Y);

            return Multiply(Multiply(matT0, matR), matT1);
       }

        internal void TransformPoints(Vector2d[] points)
        {
            foreach (Vector2d pnt in points)
            {
                MultiplyPoint(pnt);
            }
        }
        public void MultiplyPoint(Vector2d point)
        {

            double x = point.X;
            double y = point.Y;
            point.X = (((x * this.M11) + (y * this.M21)) + this.M31);
            point.Y = (((x * this.M12) + (y * this.M22)) + this.M32);
        }
    }

    public static class DoubleUtilities
    {
        private const double Epsilon = 2.2204460492503131E-50;

        public static bool IsZero(double value)
        {
            //return false;
            return (Math.Abs(value) < Epsilon);
        }

        public static bool IsOne(double value)
        {
            return (Math.Abs(value - 1) < Epsilon);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double Clamp(double x, double min, double max)
        {
            return Math.Max(min, Math.Min(x, max));
        }
    }
    public class PlaneD
    {
        public double A;
        public double B;
        public double C;
        public double D;

        public PlaneD(double valuePointA, double valuePointB, double valuePointC, double valuePointD)
        {
            this.A = valuePointA;
            this.B = valuePointB;
            this.C = valuePointC;
            this.D = valuePointD;
        }

        //public override bool Equals(object compare);
        //public static bool operator ==(Plane left, Plane right);
        // public static bool operator !=(Plane left, Plane right);
        // public override int GetHashCode();
        // public Plane();
        //  public static Plane Empty { get; }
        //  public override string ToString();
        //  public static float DotNormal(Plane p, Vector3 v);
        public void Normalize()
        {
            double length = Math.Sqrt(A * A + B * B + C * C);

            A /= length;
            B /= length;
            C /= length;
            D /= length;


            //Vector3d vector = new Vector3d(A, B, C);
            //vector.Normalize();
            //A = vector.X;
            //B = vector.Y;
            //C = vector.Z;
        }

        //   public static Plane Normalize(Plane p);
        //   public static Vector3 IntersectLine(Plane p, Vector3 v1, Vector3 v2);
        //   public static Plane FromPointNormal(Vector3 point, Vector3 normal);
        //   public static Plane FromPoints(Vector3 p1, Vector3 p2, Vector3 p3);
        //    public void Transform(Matrix m);
        //    public static Plane Transform(Plane p, Matrix m);
        //    public void Scale(float s);
        //    public static Plane Scale(Plane p, float s);
        //   public float Dot(Vector3 v);
        public double Dot(Vector4d v)
        {
            //return ((((planeRef[4] * *(((float*) (&v + 4)))) + (planeRef[8] * *(((float*) (&v + 8))))) + (planeRef[12] * *(((float*) (&v + 12))))) + (planeRef[0] * *(((float*) &v))));
            return B * v.Y + C * v.Z + D * v.W + A * v.X;
        }


    }
    public class Vector4d
    {
        public Vector4d(double valueX, double valueY, double valueZ, double valueW)
        {
            this.X = valueX;
            this.Y = valueY;
            this.Z = valueZ;
            this.W = valueW;
        }
        public double X;
        public double Y;
        public double Z;
        public double W;
    }

    public class PositionNormalTexturedX2
    {
        // Summary:
        //     Retrieves the Microsoft.DirectX.Direct3D.VertexFormats for the current custom
        //     vertex.
        //public const VertexFormats Format = VertexFormats.Position | VertexFormats.Normal | VertexFormats.Texture2;
        public double X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public double Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public double Z;
        // Summary:
        //     Retrieves or sets the nx component of the vertex normal.
        public double Nx;
        //
        // Summary:
        //     Retrieves or sets the ny component of the vertex normal.
        public double Ny;
        //
        // Summary:
        //     Retrieves or sets the nz component of the vertex normal.
        public double Nz;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public double Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public double Tv;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public double Tu1;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public double Tv1;
        //
        // Summary:
        //     Retrieves or sets the x component of the position.


        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex position.
        //
        //   nor:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex normal data.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.

        public PositionNormalTexturedX2()
        {
        }

        public static PositionNormalTexturedX2 Create2UV(Vector3d pos, Vector3d nor, double u, double v, double u1, double v1)
        {
            PositionNormalTexturedX2 temp = new PositionNormalTexturedX2();
            temp.X = pos.X;
            temp.Y = pos.Y;
            temp.Z = pos.Z;
            temp.Nx = nor.X;
            temp.Ny = nor.Y;
            temp.Nz = nor.Z;
            temp.Tu = u;
            temp.Tv = v;
            temp.Tu1 = u1;
            temp.Tv1 = v1;

            return temp;
        }

        public static PositionNormalTexturedX2 Create(Vector3d pos, Vector3d nor, double u, double v)
        {
             PositionNormalTexturedX2 temp = new PositionNormalTexturedX2();
            temp.X = pos.X;
            temp.Y = pos.Y;
            temp.Z = pos.Z;
            temp.Nx = nor.X;
            temp.Ny = nor.Y;
            temp.Nz = nor.Z;
            temp.Tu = u;
            temp.Tv = v;
            Coordinates result = Coordinates.CartesianToSpherical2(nor);
            temp.Tu1 = (float)((result.Lng + 180.0) / 360.0);
            temp.Tv1 = (float)(1 - ((result.Lat + 90.0) / 180.0));

            return temp;
        }


        public double Lat
        {
            get { return (1 - Tv1) * 180 - 90; }
            set { Tv1 = (float)(1 - ((value + 90.0) / 180.0)); }
        }

        public double Lng
        {
            get { return Tu1 * 360 - 180; }
            set { Tu1 = (float)((value + 180.0) / 360.0); }
        }


        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
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
        //   nxvalue:
        //     Floating-point value that represents the nx coordinate of the vertex normal.
        //
        //   nyvalue:
        //     Floating-point value that represents the ny coordinate of the vertex normal.
        //
        //   nzvalue:
        //     Floating-point value that represents the nz coordinate of the vertex normal.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public static PositionNormalTexturedX2 CreateLong2UV(float xvalue, float yvalue, float zvalue, float nxvalue, float nyvalue, float nzvalue, float u, float v, float u1, float v1)
        {
            PositionNormalTexturedX2 temp = new PositionNormalTexturedX2();
            temp.X = xvalue;
            temp.Y = yvalue;
            temp.Z = zvalue;
            temp.Nx = nxvalue;
            temp.Ny = nyvalue;
            temp.Nz = nzvalue;
            temp.Tu = u;
            temp.Tv = v;
            temp.Tu1 = u1;
            temp.Tv1 = v1;
            return temp;
        }
        public PositionNormalTexturedX2 CreateLong(float xvalue, float yvalue, float zvalue, float nxvalue, float nyvalue, float nzvalue, float u, float v)
        {
            PositionNormalTexturedX2 temp = new PositionNormalTexturedX2();
            temp.X = xvalue;
            temp.Y = yvalue;
            temp.Z = zvalue;
            temp.Nx = nxvalue;
            temp.Ny = nyvalue;
            temp.Nz = nzvalue;
            temp.Tu = u;
            temp.Tv = v;
            Coordinates result = Coordinates.CartesianToSpherical2(Vector3d.Create(Nx, Ny, Nz));
            temp.Tu1 = (float)((result.Lng + 180.0) / 360.0);
            temp.Tv1 = (float)(1 - ((result.Lat + 90.0) / 180.0));
            return temp;
       }
        // Summary:
        //     Retrieves or sets the vertex normal data.
        public Vector3d Normal
        {
            get
            {
                return Vector3d.Create(Nx, Ny, Nz);
            }
            set
            {
                Nx = value.X;
                Ny = value.Y;
                Nz = value.Z;
            }
        }
        //
        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position
        {
            get
            {
                return Vector3d.Create(X, Y, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }
        //
        // Summary:
        //     Retrieves the size of the PositionNormalTexturedX2
        //     structure.
        public static int StrideSize
        {
            get
            {
                return 40;
            }

        }

        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return string.Format(
                "X={0}, Y={1}, Z={2}, Nx={3}, Ny={4}, Nz={5}, U={6}, V={7}, U1={8}, U2={9}",
                X, Y, Z, Nx, Ny, Nz, Tu, Tv, Tu1, Tv1
                );
        }
    }

    public class PositionNormalTextured
    {
        // Summary:
        //     Retrieves the Microsoft.DirectX.Direct3D.VertexFormats for the current custom
        //     vertex.
        //public const VertexFormats Format = VertexFormats.Position | VertexFormats.Normal | VertexFormats.Texture2;
        public double X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public double Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public double Z;
        // Summary:
        //     Retrieves or sets the nx component of the vertex normal.
        public double Nx;
        //
        // Summary:
        //     Retrieves or sets the ny component of the vertex normal.
        public double Ny;
        //
        // Summary:
        //     Retrieves or sets the nz component of the vertex normal.
        public double Nz;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public double Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public double Tv;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
    

        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex position.
        //
        //   nor:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex normal data.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.

        public PositionNormalTextured()
        {
        }

        public static PositionNormalTextured CreateShort(Vector3d pos, Vector3d nor, double u, double v)
        {
            PositionNormalTextured temp = new PositionNormalTextured();
            temp.X = pos.X;
            temp.Y = pos.Y;
            temp.Z = pos.Z;
            temp.Nx = nor.X;
            temp.Ny = nor.Y;
            temp.Nz = nor.Z;
            temp.Tu = u;
            temp.Tv = v;
            return temp;
        }

        internal static PositionNormalTextured Create(float x, float y, float z, int nx, int ny, int nz, int tu, int tv)
        {
            PositionNormalTextured temp = new PositionNormalTextured();
            temp.X = x;
            temp.Y = y;
            temp.Z = z;
            temp.Nx = nx;
            temp.Ny = ny;
            temp.Nz = nz;
            temp.Tu = tu;
            temp.Tv = tv;
            return temp;
        }



        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
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
        //   nxvalue:
        //     Floating-point value that represents the nx coordinate of the vertex normal.
        //
        //   nyvalue:
        //     Floating-point value that represents the ny coordinate of the vertex normal.
        //
        //   nzvalue:
        //     Floating-point value that represents the nz coordinate of the vertex normal.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.

        public PositionNormalTexturedX2 CreateLong(float xvalue, float yvalue, float zvalue, float nxvalue, float nyvalue, float nzvalue, float u, float v)
        {
            PositionNormalTexturedX2 temp = new PositionNormalTexturedX2();
            temp.X = xvalue;
            temp.Y = yvalue;
            temp.Z = zvalue;
            temp.Nx = nxvalue;
            temp.Ny = nyvalue;
            temp.Nz = nzvalue;
            temp.Tu = u;
            temp.Tv = v;
            return temp;
        }


        public static PositionNormalTextured CreateUV(Vector3d pos, Vector3d nor, Vector2d uv)
        {
            PositionNormalTextured temp = new PositionNormalTextured();
            temp.X = pos.X;
            temp.Y = pos.Y;
            temp.Z = pos.Z;
            temp.Nx = nor.X;
            temp.Ny = nor.Y;
            temp.Nz = nor.Z;
            temp.Tu = uv.X;
            temp.Tv = uv.Y;
            return temp;
        }


        // Summary:
        //     Retrieves or sets the vertex normal data.
        public Vector3d Normal
        {
            get
            {
                return Vector3d.Create(Nx, Ny, Nz);
            }
            set
            {
                Nx = value.X;
                Ny = value.Y;
                Nz = value.Z;
            }
        }


        //
        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position
        {
            get
            {
                return Vector3d.Create(X, Y, Z);
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
            return string.Format(
                "X={0}, Y={1}, Z={2}, Nx={3}, Ny={4}, Nz={5}, U={6}, V={7}, U1={8}, U2={9}",
                X, Y, Z, Nx, Ny, Nz, Tu, Tv
                );
        }
    }

    public class SphereHull
    {
        public SphereHull()
        {
        }

        public Vector3d Center;
        public double Radius;

        internal static SphereHull Create(Vector3d Center, double Radius)
        {
            SphereHull temp = new SphereHull();
            temp.Center = Center;
            temp.Radius = Radius;
            return temp;
        }
    }


    public class ConvexHull
    {
        public static SphereHull FindEnclosingSphereFast(Vector3d[] points)

        {
            SphereHull result = new SphereHull();
            //Find the center of all points.

            int count = points.Length;

            Vector3d center = Vector3d.Zero;
            for (int i = 0; i <count; ++i)
            {
                center.Add(points[i]);
        
            }

            //This is the center of our sphere.
            center.Multiply(1.0 / (double)count);

            //Find the radius of the sphere

            double radius = 0f;

            for (int i = 0; i < count; ++i)
            {
                //We are doing a relative distance comparison to find the maximum distance
                //from the center of our sphere.
                double distance = Vector3d.GetLengthSq(Vector3d.SubtractVectors(points[i], center));

                if (distance > radius)
                {
                    radius = distance;
                }
            }
            

            //Find the real distance from the DistanceSquared.

            radius = Math.Sqrt(radius);

            //Construct the sphere.
            result.Center = center;
            result.Radius = radius;
            return result;
        }


        public static SphereHull FindEnclosingSphere(Vector3d[] list)
        {
            Vector3d Center = new Vector3d();
            double Radius = 0;

            int count = list.Length;
            int i;
            double dx;
            double dy;
            double dz;
            double rad_sq;
            double xspan;
            double yspan;
            double zspan;
            double maxspan;
            double old_to_p;
            double old_to_p_sq;
            double old_to_new;
            Vector3d xmin = new Vector3d();
            Vector3d xmax = new Vector3d();
            Vector3d ymin = new Vector3d();
            Vector3d ymax = new Vector3d();
            Vector3d zmin = new Vector3d();
            Vector3d zmax = new Vector3d();
            Vector3d dia1 = new Vector3d();
            Vector3d dia2 = new Vector3d();


            // FIRST PASS: find 6 minima/maxima points 
            xmin.X = ymin.Y = zmin.Z = 100000000; // initialize for min/max compare 
            xmax.X = ymax.Y = zmax.Z = -1000000000;
            for (i = 0; i < count; i++)
            {
                Vector3d current = list[i];
                // his ith point. 
                if (current.X < xmin.X)
                    xmin = current; // New xminimum point 
                if (current.X > xmax.X)
                    xmax = current;
                if (current.Y < ymin.Y)
                    ymin = current;
                if (current.Y > ymax.Y)
                    ymax = current;
                if (current.Z < zmin.Z)
                    zmin = current;
                if (current.Z > zmax.Z)
                    zmax = current;
            }
            // Set xspan = distance between the 2 points xmin & xmax (squared) 
            dx = xmax.X - xmin.X;
            dy = xmax.Y - xmin.Y;
            dz = xmax.Z - xmin.Z;
            xspan = dx * dx + dy * dy + dz * dz;

            // Same for y & z spans 
            dx = ymax.X - ymin.X;
            dy = ymax.Y - ymin.Y;
            dz = ymax.Z - ymin.Z;
            yspan = dx * dx + dy * dy + dz * dz;

            dx = zmax.X - zmin.X;
            dy = zmax.Y - zmin.Y;
            dz = zmax.Z - zmin.Z;
            zspan = dx * dx + dy * dy + dz * dz;

            dia1 = xmin; // assume xspan biggest 
            dia2 = xmax;
            maxspan = xspan;
            if (yspan > maxspan)
            {
                maxspan = yspan;
                dia1 = ymin;
                dia2 = ymax;
            }
            if (zspan > maxspan)
            {
                dia1 = zmin;
                dia2 = zmax;
            }


            // dia1,dia2 is a diameter of initial sphere 
            // calc initial center 
            Center.X = (dia1.X + dia2.X) / 2.0;
            Center.Y = (dia1.Y + dia2.Y) / 2.0;
            Center.Z = (dia1.Z + dia2.Z) / 2.0;
            // calculate initial radius**2 and radius 
            dx = dia2.X - Center.X; // x component of radius vector 
            dy = dia2.Y - Center.Y; // y component of radius vector 
            dz = dia2.Z - Center.Z; // z component of radius vector 
            rad_sq = dx * dx + dy * dy + dz * dz;
            Radius = Math.Sqrt(rad_sq);

            // SECOND PASS: increment current sphere 

            for (i = 0; i < count; i++)
            {
                Vector3d current = list[i]; // load global struct caller_p 
                // with his ith point. 
                dx = current.X - Center.X;
                dy = current.Y - Center.Y;
                dz = current.Z - Center.Z;
                old_to_p_sq = dx * dx + dy * dy + dz * dz;
                if (old_to_p_sq > rad_sq) // do r**2 test first 
                { // this point is outside of current sphere 
                    old_to_p = Math.Sqrt(old_to_p_sq);
                    // calc radius of new sphere 
                    Radius = (Radius + old_to_p) / 2.0;
                    rad_sq = Radius * Radius; // for next r**2 compare 
                    old_to_new = old_to_p - Radius;
                    // calc center of new sphere 
                    Center.X = (Radius * Center.X + old_to_new * current.X) / old_to_p;
                    Center.Y = (Radius * Center.Y + old_to_new * current.Y) / old_to_p;
                    Center.Z = (Radius * Center.Z + old_to_new * current.Z) / old_to_p;
                    // Suppress if desired 
                    //Console.Write("\n New sphere: cen,rad = {0:f} {1:f} {2:f}   {3:f}", cen.X, cen.Y, cen.Z, rad);
                }
            }

            return SphereHull.Create(Center, Radius);

        }// end of find_bounding_sphere() 

        //public static void FindEnclosingCircle(Vector2d[] list, out Vector2d cen, out double rad)
        //{
        //    cen = new Vector2d();
        //    int count = list.Length;
        //    int i;
        //    double dx;
        //    double dy;
        //    double rad_sq;
        //    double xspan;
        //    double yspan;
        //    double maxspan;
        //    double old_to_p;
        //    double old_to_p_sq;
        //    double old_to_new;
        //    Vector2d xmin = new Vector2d();
        //    Vector2d xmax = new Vector2d();
        //    Vector2d ymin = new Vector2d();
        //    Vector2d ymax = new Vector2d();
        //    Vector2d dia1 = new Vector2d();
        //    Vector2d dia2 = new Vector2d();


        //    // FIRST PASS: find 6 minima/maxima points 
        //    xmin.X = ymin.Y = 100000000; // initialize for min/max compare 
        //    xmax.X = ymax.Y = -1000000000;
        //    for (i = 0; i < count; i++)
        //    {
        //        Vector2d current = list[i];
        //        // his ith point. 
        //        if (current.X < xmin.X)
        //            xmin = current; // New xminimum point 
        //        if (current.X > xmax.X)
        //            xmax = current;
        //        if (current.Y < ymin.Y)
        //            ymin = current;
        //        if (current.Y > ymax.Y)
        //            ymax = current;

        //    }
        //    // Set xspan = distance between the 2 points xmin & xmax (squared) 
        //    dx = xmax.X - xmin.X;
        //    dy = xmax.Y - xmin.Y;
        //    xspan = dx * dx + dy * dy;

        //    // Same for y & z spans 
        //    dx = ymax.X - ymin.X;
        //    dy = ymax.Y - ymin.Y;
        //    yspan = dx * dx + dy * dy;

        //    dia1 = xmin; // assume xspan biggest 
        //    dia2 = xmax;
        //    maxspan = xspan;
        //    if (yspan > maxspan)
        //    {
        //        maxspan = yspan;
        //        dia1 = ymin;
        //        dia2 = ymax;
        //    }


        //    // dia1,dia2 is a diameter of initial sphere 
        //    // calc initial center 
        //    cen.X = (dia1.X + dia2.X) / 2.0;
        //    cen.Y = (dia1.Y + dia2.Y) / 2.0;
        //    // calculate initial radius**2 and radius 
        //    dx = dia2.X - cen.X; // x component of radius vector 
        //    dy = dia2.Y - cen.Y; // y component of radius vector 
        //    rad_sq = dx * dx + dy * dy;
        //    rad = Math.Sqrt(rad_sq);

        //    // SECOND PASS: increment current sphere 

        //    for (i = 0; i < count; i++)
        //    {
        //        Vector2d current = list[i]; // load global struct caller_p 
        //        // with his ith point. 
        //        dx = current.X - cen.X;
        //        dy = current.Y - cen.Y;
        //        old_to_p_sq = dx * dx + dy * dy;
        //        if (old_to_p_sq > rad_sq) // do r**2 test first 
        //        { // this point is outside of current sphere 
        //            old_to_p = Math.Sqrt(old_to_p_sq);
        //            // calc radius of new sphere 
        //            rad = (rad + old_to_p) / 2.0;
        //            rad_sq = rad * rad; // for next r**2 compare 
        //            old_to_new = old_to_p - rad;
        //            // calc center of new sphere 
        //            cen.X = (rad * cen.X + old_to_new * current.X) / old_to_p;
        //            cen.Y = (rad * cen.Y + old_to_new * current.Y) / old_to_p;
        //        }
        //    }
        //}// end of find_bounding_circle() 
    }
}
