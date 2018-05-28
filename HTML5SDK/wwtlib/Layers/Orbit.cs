using System;
using System.Collections.Generic;
using System.Linq;
using System.Html;
using System.Xml;
using System.Html.Services;
using System.Html.Media.Graphics;
namespace wwtlib
{
    public class Orbit
    {
        private EOE elements = null;

        Color orbitColor = Colors.White;
        float scale;
        public Orbit(EOE elements, int segments, Color color, float thickness, float scale)
        {
            this.elements = elements;
            this.segmentCount = segments;
            this.orbitColor = color;
            this.scale = scale;
        }
        public void CleanUp()
        {

        }

        // Get the radius of a sphere (centered at a focus of the ellipse) that is
        // large enough to contain the orbit. The value returned has units of the orbit scale.
        public double BoundingRadius
        {
            get
            {
                if (elements != null)
                {
                    return (elements.a * (1.0 + elements.e)) / scale;
                }
                else
                {
                    return 0.0;
                }
            }
        }

        // Convert from standard coordinate system with z normal to the orbital plane
        // to WWT's system where y is the normal. Note that this transformation is not
        // a pure rotation: it incorporates a reflection, because the two systems have
        // different handedness.
        static Matrix3d orbitalToWwt = Matrix3d.Create(1.0, 0.0, 0.0, 0.0,
                                                    0.0, 0.0, 1.0, 0.0,
                                                    0.0, 1.0, 0.0, 0.0,
                                                    0.0, 0.0, 0.0, 1.0);

        static bool initBegun = false;
        // ** Begin 
        public void Draw3D(RenderContext renderContext, float opacity, Vector3d centerPoint)
        {
            Matrix3d orbitalPlaneOrientation = Matrix3d.MultiplyMatrix(Matrix3d.RotationZ(Coordinates.DegreesToRadians(elements.w)),
                                                         Matrix3d.MultiplyMatrix( Matrix3d.RotationX(Coordinates.DegreesToRadians(elements.i)),
                                                         Matrix3d.RotationZ(Coordinates.DegreesToRadians(elements.omega))));

            // Extra transformation required because the ellipse shader uses the xy-plane, but WWT uses the
            // xz-plane as the reference.
            orbitalPlaneOrientation = Matrix3d.MultiplyMatrix(orbitalPlaneOrientation, orbitalToWwt);

            Matrix3d worldMatrix = Matrix3d.MultiplyMatrix( Matrix3d.MultiplyMatrix(orbitalPlaneOrientation , Matrix3d.Translation(centerPoint)), renderContext.World);

            double M = elements.n * (SpaceTimeController.JNow - elements.T);
            double F = 1;
            if (M < 0)
            {
                F = -1;
            }
            M = Math.Abs(M) / 360.0;
            M = (M - (int)(M)) * 360.0 * F;

            Color color = Color.FromArgbColor((int)(opacity * 255.0f), orbitColor);

            // Newton-Raphson iteration to solve Kepler's equation.
            // This is faster than calling CAAKepler.Calculate(), and 5 steps
            // is more than adequate for draw the orbit paths of small satellites
            // (which are ultimately rendered using single-precision floating point.)
            M = Coordinates.DegreesToRadians(M);
            double E = M;
            for (int i = 0; i < 5; i++)
            {
                E += (M - E + elements.e * Math.Sin(E)) / (1 - elements.e * Math.Cos(E));
            }

            EllipseRenderer.DrawEllipse(renderContext, elements.a / scale, elements.e, E, color, worldMatrix);
        }

        //VertexBuffer orbitVertexBuffer = null;
        int segmentCount = 0;
    }

    public class EllipseRenderer
    {
        private static PositionVertexBuffer ellipseVertexBuffer;
        private static PositionVertexBuffer ellipseWithoutStartPointVertexBuffer;
        private static EllipseShader ellipseShader;


        // Draw an ellipse with the specified semi-major axis and eccentricity. The orbit is drawn over a single period,
        // fading from full brightness at the given eccentric anomaly.
        //
        // In order to match exactly the position at which a planet is drawn, the planet's position at the current time
        // must be passed as a parameter. positionNow is in the current coordinate system of the render context, not the
        // translated and rotated system of the orbital plane.
        public static void DrawEllipseWithPosition(RenderContext renderContext, double semiMajorAxis, double eccentricity, double eccentricAnomaly, Color color, Matrix3d worldMatrix, Vector3d positionNow)
        {
            if (ellipseShader == null)
            {
                ellipseShader = new EllipseShader();
            }

            if (ellipseVertexBuffer == null)
            {
                ellipseVertexBuffer = CreateEllipseVertexBuffer(500);
            }

            Matrix3d savedWorld = renderContext.World;
            renderContext.World = worldMatrix;

            renderContext.gl.bindBuffer(GL.ARRAY_BUFFER, ellipseVertexBuffer.VertexBuffer);
            renderContext.gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
           
            EllipseShader.Use(renderContext, (float)semiMajorAxis, (float)eccentricity, (float)eccentricAnomaly, color, 1.0f, savedWorld, positionNow);

            renderContext.gl.drawArrays(GL.LINE_STRIP, 0, ellipseVertexBuffer.Count);

            renderContext.World = savedWorld;
        }


        // This version of DrawEllipse works without a 'head' point
        public static void DrawEllipse(RenderContext renderContext, double semiMajorAxis, double eccentricity, double eccentricAnomaly, Color color, Matrix3d worldMatrix)
        {
            if (ellipseShader == null)
            {
                ellipseShader = new EllipseShader();
            }

            if (ellipseWithoutStartPointVertexBuffer == null)
            {
                ellipseWithoutStartPointVertexBuffer = CreateEllipseVertexBufferWithoutStartPoint(360);
            }

            Matrix3d savedWorld = renderContext.World;
            renderContext.World = worldMatrix;

            renderContext.gl.bindBuffer(GL.ARRAY_BUFFER, ellipseWithoutStartPointVertexBuffer.VertexBuffer);
            renderContext.gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
            EllipseShader.Use(renderContext, (float)semiMajorAxis, (float)eccentricity, (float)eccentricAnomaly, color, 1.0f, savedWorld, Vector3d.Create(0.0, 0.0, 0.0));

            renderContext.gl.drawArrays(GL.LINE_STRIP, 0, ellipseWithoutStartPointVertexBuffer.Count-1);

            renderContext.World = savedWorld;
        }


        public static PositionVertexBuffer CreateEllipseVertexBuffer(int vertexCount)
        {
            PositionVertexBuffer vb = new PositionVertexBuffer(vertexCount);
            Vector3d[] verts = vb.Lock();
            int index = 0;
            // Pack extra samples into the front of the orbit to avoid obvious segmentation
            // when viewed from near the planet or moon.
            for (int i = 0; i < vertexCount / 2; ++i)
            {
                verts[index++] = Vector3d.Create(2.0f * (float)i / (float)vertexCount * 0.05f, 0.0f, 0.0f);
            }
            for (int i = 0; i < vertexCount / 2; ++i)
            {
                verts[index++] = Vector3d.Create(2.0f * (float)i / (float)vertexCount * 0.95f + 0.05f, 0.0f, 0.0f);
            }

            vb.Unlock();

            return vb;
        }

        public static PositionVertexBuffer CreateEllipseVertexBufferWithoutStartPoint(int vertexCount)
        {
            PositionVertexBuffer vb = new PositionVertexBuffer(vertexCount);
            Vector3d[] verts = vb.Lock();

            // Setting a non-zero value will prevent the ellipse shader from using the 'head' point
            verts[0] = Vector3d.Create(1.0e-6f, 0.0f, 0.0f);

            for (int i = 1; i < vertexCount; ++i)
            {
                verts[i] = Vector3d.Create(2.0f * (float)i / (float)vertexCount, 0.0f, 0.0f);
            }

            vb.Unlock();

            return vb;
        }
    }
}
