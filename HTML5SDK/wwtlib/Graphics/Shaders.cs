using System;
using System.Collections.Generic;
using System.Html;


namespace wwtlib
{

    public class SimpleLineShader
    {
        public SimpleLineShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static WebGLUniformLocation lineColorLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      "   precision highp float;                                                          \n" +
                      "   uniform vec4 lineColor;                                                         \n" +
                      "                                                                                   \n" +
                      "   void main(void) {                                                               \n" +
                      "       gl_FragColor = lineColor;                                                   \n" +
                      "   }                                                                               \n";


            String vertexShaderText =
                      "     attribute vec3 aVertexPosition;                                              \n" +
                      "                                                                                  \n" +
                      "     uniform mat4 uMVMatrix;                                                      \n" +
                      "     uniform mat4 uPMatrix;                                                       \n" +
                      "                                                                                  \n" +
                      "                                                                                  \n" +
                      "                                                                                  \n" +
                      "     void main(void) {                                                            \n" +
                      "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                      "     }                                                                            \n" +
                      "                                                                                  \n";
            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);


            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            lineColorLoc = gl.getUniformLocation(prog, "lineColor");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex, Color lineColor, bool useDepth)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                gl.uniform4f(lineColorLoc, lineColor.R/255, lineColor.G/255, lineColor.B/255, 1);
                if (renderContext.Space || !useDepth)
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);


                gl.enableVertexAttribArray(vertLoc);
                
                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 0, 0);
                gl.lineWidth(1.0f);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            }
        }
    }

    public class OrbitLineShader
    {
        public OrbitLineShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int colorLoc;
        public static WebGLUniformLocation lineColorLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      "    precision highp float;                                                        \n" +
                      "    uniform vec4 lineColor;                                                       \n" +
                      "    varying lowp vec4 vColor;                                                     \n" +
                      "                                                                                  \n" +
                      "    void main(void) {                                                             \n" +
                      "        gl_FragColor = lineColor * vColor;                                        \n" +
                      "    }                                                                             \n";


            String vertexShaderText =
                      "     attribute vec3 aVertexPosition;                                              \n" +
                      "     attribute vec4 aVertexColor;                                                 \n" +
                      "                                                                                  \n" +
                      "     uniform mat4 uMVMatrix;                                                      \n" +
                      "     uniform mat4 uPMatrix;                                                       \n" +
                      "     varying lowp vec4 vColor;                                                    \n" +
                      "                                                                                  \n" +
                      "                                                                                  \n" +
                      "                                                                                  \n" +
                      "     void main(void) {                                                            \n" +
                      "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                      "         vColor = aVertexColor;                                                   \n" +
                      "     }                                                                            \n" +
                      "                                                                                  \n";
            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);


            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            colorLoc = gl.getAttribLocation(prog, "aVertexColor");
            lineColorLoc = gl.getUniformLocation(prog, "lineColor");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex, Color lineColor)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                gl.uniform4f(lineColorLoc, lineColor.R / 255, lineColor.G / 255, lineColor.B / 255, 1);
                if (renderContext.Space)
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);



                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(colorLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 28, 0);
                gl.vertexAttribPointer(colorLoc, 4, GL.FLOAT, false, 28, 12);
                gl.lineWidth(1.0f);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            }
        }
    }


    public class LineShaderNormalDates
    {
        public LineShaderNormalDates()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int colorLoc;
        public static int timeLoc;
        public static WebGLUniformLocation lineColorLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation jNowLoc;
        public static WebGLUniformLocation decayLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                    "    precision highp float;                                                              \n" +
                    "    uniform vec4 lineColor;                                                             \n" +
                    "    varying lowp vec4 vColor;                                                           \n" +
                    "    void main(void)                                                                     \n" +
                    "    {                                                                                   \n" +
                    "        gl_FragColor = lineColor * vColor;                                              \n" +
                    "    }                                                                                   \n";


            String vertexShaderText =
                    "    attribute vec3 aVertexPosition;                                                     \n" +
                    "    attribute vec4 aVertexColor;                                                        \n" +
                    "    attribute vec2 aTime;                                                               \n" +
                    "    uniform mat4 uMVMatrix;                                                             \n" +
                    "    uniform mat4 uPMatrix;                                                              \n" +
                    "    uniform float jNow;                                                                 \n" +
                    "    uniform float decay;                                                                \n" +
                    "                                                                                        \n" +
                    "    varying lowp vec4 vColor;                                                           \n" +
                    "                                                                                        \n" +
                    "    void main(void)                                                                     \n" +
                    "    {                                                                                   \n" +
                    "        gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);                \n" +
                    "        float dAlpha = 1.0;                                                             \n" +
                    "        if ( decay > 0.0)                                                               \n" +
                    "        {                                                                               \n" +
                    "             dAlpha = 1.0 - ((jNow - aTime.y) / decay);                                 \n " +
                    "             if (dAlpha > 1.0 )                                                         \n" +
                    "             {                                                                          \n" +
                    "                  dAlpha = 1.0;                                                         \n" +
                    "             }                                                                          \n" +
                    "        }                                                                               \n" +
                    "        if (jNow < aTime.x && decay > 0.0)                                              \n" +
                    "        {                                                                               \n" +
                    "            vColor = vec4(1, 1, 1, 1);                                                  \n" +
                    "        }                                                                               \n" +
                    "        else                                                                            \n" +
                    "        {                                                                               \n" +
                    "           vColor = vec4(aVertexColor.r, aVertexColor.g, aVertexColor.b, dAlpha * aVertexColor.a);          \n" +
                    "        }                                                                                \n" +
                    "    }                                                                                    \n" +
                    "                                                                                         \n";


            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);


            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            colorLoc = gl.getAttribLocation(prog, "aVertexColor");
            timeLoc = gl.getAttribLocation(prog, "aTime");
            lineColorLoc = gl.getUniformLocation(prog, "lineColor");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            jNowLoc = gl.getUniformLocation(prog, "jNow");
            decayLoc = gl.getUniformLocation(prog, "decay");

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex, Color lineColor, bool zBuffer, float jNow, float decay)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                gl.uniform4f(lineColorLoc, lineColor.R / 255, lineColor.G / 255, lineColor.B / 255, 1);
                gl.uniform1f(jNowLoc, jNow);
                gl.uniform1f(decayLoc, decay);

                if (zBuffer)
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);



                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(colorLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 36, 0);
                gl.vertexAttribPointer(colorLoc, 4, GL.FLOAT, false, 36, 12);
                gl.vertexAttribPointer(timeLoc, 2, GL.FLOAT, false, 36, 28);
                gl.lineWidth(1.0f);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            }
        }
    }


    public class TimeSeriesPointSpriteShader
    {
        public TimeSeriesPointSpriteShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int colorLoc;
        public static int pointSizeLoc;
        public static int timeLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation sampLoc;
        public static WebGLUniformLocation jNowLoc;
        public static WebGLUniformLocation decayLoc;
        public static WebGLUniformLocation lineColorLoc;
        public static WebGLUniformLocation cameraPosLoc;
        public static WebGLUniformLocation scaleLoc;
        public static WebGLUniformLocation minSizeLoc;
        public static WebGLUniformLocation skyLoc;
        public static WebGLUniformLocation showFarSideLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                    "    precision mediump float;                                                            \n" +
                    "    uniform vec4 lineColor;                                                             \n" +
                    "    varying lowp vec4 vColor;                                                           \n" +
                    "    uniform sampler2D uSampler;                                                         \n" +
                    "    void main(void)                                                                     \n" +
                    "    {                                                                                   \n" +
                    "        vec4 texColor;                                                                  \n" +
                    "        texColor = texture2D(uSampler, gl_PointCoord);                                  \n" +
                    "                                                                                        \n" +
                    "                                                                                        \n" +
                    "        gl_FragColor = lineColor * vColor * texColor;                                   \n" +
                    "    }                                                                                   \n";


            String vertexShaderText =
                    "    attribute vec3 aVertexPosition;                                                     \n" +
                    "    attribute vec4 aVertexColor;                                                        \n" +
                    "    attribute vec2 aTime;                                                               \n" +
                    "    attribute float aPointSize;                                                         \n" +
                    "    uniform mat4 uMVMatrix;                                                             \n" +
                    "    uniform mat4 uPMatrix;                                                              \n" +
                    "    uniform float jNow;                                                                 \n" +
                    "    uniform vec3 cameraPosition;                                                        \n" +
                    "    uniform float decay;                                                                \n" +
                    "    uniform float scale;                                                                \n" +
                    "    uniform float minSize;                                                              \n" +
                    "    uniform float sky;                                                                  \n" +
                    "    uniform float showFarSide;                                                          \n" +
                    "                                                                                        \n" +
                    "    varying lowp vec4 vColor;                                                           \n" +
                    "                                                                                        \n" +
                    "    void main(void)                                                                     \n" +
                    "    {                                                                                   \n" +
                    "        float dotCam = dot( normalize(cameraPosition-aVertexPosition), normalize(aVertexPosition));                                  \n" +
                    "        float dist = distance(aVertexPosition, cameraPosition);                         \n" +
                    "        gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);                \n" +
                    "        float dAlpha = 1.0;                                                             \n" +
                    "        if ( decay > 0.0)                                                               \n" +
                    "        {                                                                               \n" +
                    "             dAlpha = 1.0 - ((jNow - aTime.y) / decay);                                 \n " +
                    "             if (dAlpha > 1.0 )                                                         \n" +
                    "             {                                                                          \n" +
                    "                  dAlpha = 1.0;                                                         \n" +
                    "             }                                                                          \n" +
                    "        }                                                                               \n" +
                    "        if ( showFarSide == 0.0 && (dotCam * sky) < 0.0 || (jNow < aTime.x && decay > 0.0))                                              \n" +
                    "        {                                                                               \n" +
                    "            vColor = vec4(0.0, 0.0, 0.0, 0.0);                                          \n" +
                    "        }                                                                               \n" +
                    "        else                                                                            \n" +
                    "        {                                                                               \n" +
                    "           vColor = vec4(aVertexColor.r, aVertexColor.g, aVertexColor.b, dAlpha);       \n" +
                    "        }                                                                               \n" +
                    "        float lSize = scale;                                                            \n" +
                    "        if (scale < 0.0)                                                                \n" +
                    "        {                                                                               \n" +
                    "           lSize = -scale;                                                              \n" +
                    "           dist = 1.0;                                                                  \n" +
                    "        }                                                                               \n" +
                    "        gl_PointSize = max(minSize, (lSize * ( aPointSize ) / dist));                   \n" +
                    "    }                                                                                   \n" +
                    "                                                                                        \n";


            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);


            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);
            object compilationLog = gl.getShaderInfoLog(vert);
            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            colorLoc = gl.getAttribLocation(prog, "aVertexColor");
            pointSizeLoc = gl.getAttribLocation(prog, "aPointSize");
            timeLoc = gl.getAttribLocation(prog, "aTime");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            sampLoc = gl.getUniformLocation(prog, "uSampler");
            jNowLoc = gl.getUniformLocation(prog, "jNow");
            decayLoc = gl.getUniformLocation(prog, "decay");
            lineColorLoc = gl.getUniformLocation(prog, "lineColor");
            cameraPosLoc = gl.getUniformLocation(prog, "cameraPosition");
            scaleLoc = gl.getUniformLocation(prog, "scale");
            skyLoc = gl.getUniformLocation(prog, "sky");
            showFarSideLoc = gl.getUniformLocation(prog, "showFarSide");
            minSizeLoc = gl.getUniformLocation(prog, "minSize");

            gl.enable(GL.BLEND);
           
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLTexture texture, Color lineColor, bool zBuffer, float jNow, float decay, Vector3d camera, float scale, float minSize, bool showFarSide, bool sky)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                gl.uniform1i(sampLoc, 0);
                gl.uniform1f(jNowLoc, jNow);
                gl.uniform1f(decayLoc, decay);
                gl.uniform4f(lineColorLoc, lineColor.R / 255f, lineColor.G / 255f, lineColor.B / 255f, lineColor.A / 255f);
                gl.uniform3f(cameraPosLoc, (float)camera.X, (float)camera.Y, (float)camera.Z);
                gl.uniform1f(scaleLoc, scale);
                gl.uniform1f(minSizeLoc, minSize);
                gl.uniform1f(showFarSideLoc, showFarSide ? 1 : 0);
                gl.uniform1f(skyLoc, sky? -1 : 1);
                if (zBuffer)
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                //gl.enable(0x8642);
                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);

                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(colorLoc);
                gl.enableVertexAttribArray(pointSizeLoc);
                gl.enableVertexAttribArray(timeLoc);

                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 40, 0);
                gl.vertexAttribPointer(colorLoc, 4, GL.FLOAT, false, 40, 12);
                gl.vertexAttribPointer(pointSizeLoc, 1, GL.FLOAT, false, 40, 36);
                gl.vertexAttribPointer(timeLoc, 2, GL.FLOAT, false, 40, 28);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.lineWidth(1.0f);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE);
            }
        }
    }

    public class KeplerPointSpriteShader
    {
        public KeplerPointSpriteShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;

        public static int ABCLoc;
        public static int abcLoc1;
        public static int pointSizeLoc;
        public static int colorLoc;
        public static int weLoc;
        public static int nTLoc;
        public static int azLoc;
        public static int orbitLoc;

        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation jNowLoc;
        public static WebGLUniformLocation cameraPosLoc;
        public static WebGLUniformLocation mmLoc;
        public static WebGLUniformLocation lineColorLoc;
        public static WebGLUniformLocation scaleLoc;
        public static WebGLUniformLocation minSizeLoc;
        public static WebGLUniformLocation sampLoc;
        public static WebGLUniformLocation opacityLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                    "    precision mediump float;                                                            \n" +
                    "    uniform vec4 lineColor;                                                             \n" +
                    "    varying lowp vec4 vColor;                                                           \n" +
                    "    uniform sampler2D uSampler;                                                         \n" +
                    "    void main(void)                                                                     \n" +
                    "    {                                                                                   \n" +
                    "        vec4 texColor;                                                                  \n" +
                    "        texColor = texture2D(uSampler, gl_PointCoord);                                  \n" +
                    "                                                                                        \n" +
                    "                                                                                        \n" +
                    "        gl_FragColor = lineColor * vColor * texColor;                                   \n" +
                    "    }                                                                                   \n";


            String vertexShaderText =
                    "    attribute vec3 ABC;                                                                 \n" +
                    "    attribute vec3 abc;                                                                 \n" +
                    "    attribute float PointSize;                                                          \n" +
                    "    attribute vec4 Color;                                                               \n" +
                    "    attribute vec2 we;                                                                  \n" +
                    "    attribute vec2 nT;                                                                  \n" +
                    "    attribute vec2 az;                                                                  \n" +
                    "    attribute vec2 orbit;                                                               \n" +
                    "    uniform mat4 uMVMatrix;                                                             \n" +
                    "    uniform mat4 uPMatrix;                                                              \n" +
                    "    uniform float jNow;                                                                 \n" +
                    "    uniform vec3 cameraPosition;                                                        \n" +
                    "    uniform float MM;                                                                   \n" +
                    "    uniform float scaling;                                                              \n" +
                    "    uniform float minSize;                                                              \n" +
                    "    uniform float opacity;                                                              \n" +
                    "    varying lowp vec4 vColor;                                                           \n" +
                    "                                                                                        \n" +
                    "    void main(void)                                                                     \n" +
                    "    {                                                                                   \n" +
                    "     float M = nT.x * (jNow - nT.y) * 0.01745329251994;                                 \n" +
                    "     float e = we.y;                                                                    \n" +
                    "     float a = az.x;                                                                    \n" +
                    "     float PI = 3.1415926535897932384;                                                  \n" +
                    "     float w = we.x* 0.01745329251994;                                                  \n" +
                    "     float F = 1.0;                                                                     \n" +
                    "     if (M < 0.0)                                                                       \n" +
                    "       F = -1.0;                                                                        \n" +
                    "     M = abs(M) / (2.0 * PI);                                                           \n" +
                    "     M = (M - float(int(M)))*2.0 *PI *F;                                                \n" +
                    "     if (MM != 0.0)                                                                     \n" +
                    "     {                                                                                  \n" +
                    "       M = MM + (1.0- orbit.x) *2.0 *PI;                                                \n" +
                    "       if (M > (2.0*PI))                                                                \n" +
                    "           M = M - (2.0*PI);                                                            \n" +
                    "     }                                                                                  \n" +
                    "                                                                                        \n" +
                    "     if (M < 0.0)                                                                       \n" +
                    "       M += 2.0 *PI;                                                                    \n" +
                    "     F = 1.0;                                                                           \n" +
                    "     if (M > PI)                                                                        \n" +
                    "        F = -1.0;                                                                       \n" +
                    "     if (M > PI)                                                                        \n" +
                    "       M = 2.0 *PI - M;                                                                 \n" +
                    "                                                                                        \n" +
                    "     float E = PI / 2.0;                                                                \n" +
                    "     float scale = PI / 4.0;                                                            \n" +
                    "     for (int i =0; i<23; i++)                                                          \n" +
                    "     {                                                                                  \n" +
                    "       float R = E - e *sin(E);                                                         \n" +
                    "       if (M > R)                                                                       \n" +
                    "      	E += scale;                                                                      \n" +
                    "       else                                                                             \n" +
                    "     	E -= scale;                                                                      \n" +
                    "       scale /= 2.0;                                                                    \n" +
                    "     }                                                                                  \n" +
                    "      E = E * F;                                                                        \n" +
                    "                                                                                        \n" +
                    "     float v = 2.0 * atan(sqrt((1.0 + e) / (1.0 -e )) * tan(E/2.0));                    \n" +
                    "     float r = a * (1.0-e * cos(E));                                                    \n" +
                    "                                                                                        \n" +
                    "     vec4 pnt;                                                                          \n" +
                    "     pnt.x = r * abc.x * sin(ABC.x + w + v);                                            \n" +
                    "     pnt.z = r * abc.y * sin(ABC.y + w + v);                                            \n" +
                    "     pnt.y = r * abc.z * sin(ABC.z + w + v);                                            \n" +
                    "     pnt.w = 1.0;                                                                       \n" +
                    "                                                                                        \n" +
                    "     float dist = distance(pnt.xyz, cameraPosition.xyz);                                \n" +
                    "     gl_Position = uPMatrix * uMVMatrix * pnt;                                          \n" + 
                    "     vColor.a = opacity * (1.0-(orbit.x));                                              \n" +
                    "     vColor.r = Color.r;                                                                \n" +
                    "     vColor.g = Color.g;                                                                \n" +
                    "     vColor.b = Color.b;                                                                \n" +
                    "     gl_PointSize = max(minSize, scaling * (PointSize / dist));                         \n" +
                    " }                                                                                      \n";



            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);


            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);
            object compilationLog = gl.getShaderInfoLog(vert);
            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            abcLoc1 = gl.getAttribLocation(prog, "abc");
            ABCLoc = gl.getAttribLocation(prog, "ABC");
            pointSizeLoc = gl.getAttribLocation(prog, "PointSize");
            colorLoc = gl.getAttribLocation(prog, "Color");
            weLoc = gl.getAttribLocation(prog, "we");
            nTLoc = gl.getAttribLocation(prog, "nT");
            azLoc = gl.getAttribLocation(prog, "az");
            orbitLoc = gl.getAttribLocation(prog, "orbit");


            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            jNowLoc = gl.getUniformLocation(prog, "jNow");
            cameraPosLoc = gl.getUniformLocation(prog, "cameraPosition");
            mmLoc = gl.getUniformLocation(prog, "MM");
            scaleLoc = gl.getUniformLocation(prog, "scaling");
            minSizeLoc = gl.getUniformLocation(prog, "minSize");
            lineColorLoc = gl.getUniformLocation(prog, "lineColor");
            opacityLoc = gl.getUniformLocation(prog, "opacity");
            sampLoc = gl.getUniformLocation(prog, "uSampler");
            gl.enable(GL.BLEND);

            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, Matrix3d worldView, WebGLBuffer vertex, WebGLTexture texture, Color lineColor, float opacity, bool zBuffer, float jNow, float MM, Vector3d camera, float scale, float minSize)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                gl.uniformMatrix4fv(mvMatLoc, false, worldView.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                gl.uniform1i(sampLoc, 0);
                gl.uniform1f(jNowLoc, jNow);
                gl.uniform1f(mmLoc, MM);
                gl.uniform4f(lineColorLoc, lineColor.R / 255f, lineColor.G / 255f, lineColor.B / 255f, lineColor.A / 255f);
                gl.uniform1f(opacityLoc, opacity);
                gl.uniform3f(cameraPosLoc, (float)camera.X, (float)camera.Y, (float)camera.Z);
                gl.uniform1f(scaleLoc, scale);
                gl.uniform1f(minSizeLoc, minSize);
                if (zBuffer)
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.disable(GL.DEPTH_TEST);
                }

                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);

                gl.enableVertexAttribArray(ABCLoc);
                gl.enableVertexAttribArray(abcLoc1);
                gl.enableVertexAttribArray(colorLoc);
                gl.enableVertexAttribArray(pointSizeLoc);
                gl.enableVertexAttribArray(weLoc);
                gl.enableVertexAttribArray(nTLoc);
                gl.enableVertexAttribArray(azLoc);
                gl.enableVertexAttribArray(orbitLoc);

                gl.enableVertexAttribArray(weLoc);
                gl.vertexAttribPointer(ABCLoc, 3, GL.FLOAT, false, 76, 0);
                gl.vertexAttribPointer(abcLoc1, 3, GL.FLOAT, false, 76, 12);
                gl.vertexAttribPointer(pointSizeLoc, 1, GL.FLOAT, false, 76, 24);
                gl.vertexAttribPointer(colorLoc, 4, GL.FLOAT, false, 76, 28);
                gl.vertexAttribPointer(weLoc, 2, GL.FLOAT, false, 76, 44);
                gl.vertexAttribPointer(nTLoc, 2, GL.FLOAT, false, 76, 52);
                gl.vertexAttribPointer(azLoc, 2, GL.FLOAT, false, 76, 60);
                gl.vertexAttribPointer(orbitLoc, 2, GL.FLOAT, false, 76, 68);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.lineWidth(1.0f);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE);
            }
        }
    }

    public class EllipseShader
    {
        public EllipseShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;

        public static int AngleLoc;


        public static WebGLUniformLocation matWVPLoc;
        public static WebGLUniformLocation matPositionLoc;
        public static WebGLUniformLocation positionNowLoc;
        public static WebGLUniformLocation colorLoc;
        public static WebGLUniformLocation opacityLoc;
        public static WebGLUniformLocation semiMajorAxisLoc;
        public static WebGLUniformLocation eccentricityLoc;
        public static WebGLUniformLocation eccentricAnomalyLoc;


        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                    "    precision mediump float;                                                            \n" +
                    "    varying lowp vec4 vColor;                                                           \n" +
                    "    void main(void)                                                                     \n" +
                    "    {                                                                                   \n" +
                    "        gl_FragColor = vColor;                                          \n" +
                    "    }                                                                                   \n";


            String vertexShaderText =
                    "    attribute vec3 Angle;                                                               \n" +
                    "    uniform mat4 matWVP;                                                             \n" +
                    "    uniform mat4 matPosition;                                                              \n" +
                    "    uniform vec3 positionNow;                                                        \n" +
                    "    uniform float semiMajorAxis;                                                                   \n" +
                    "    uniform float eccentricity;                                                              \n" +
                    "    uniform vec4 color;                                                             \n" +
                    "    uniform float eccentricAnomaly;                                                              \n" +
         //           "    uniform float opacity;                                                              \n" +
                    "    varying lowp vec4 vColor;                                                           \n" +
                    "                                                                                        \n" +
                    "    void main(void)                                                                     \n" +
                    "    {                                                                                   \n" +
                    "        float fade = (1.0 - Angle.x);                                                    \n" +
                    "        float PI = 3.1415927;                                                          \n" +
                    "        float E = eccentricAnomaly - Angle.x * 2.0 * PI;                                   \n" +
                    "        vec2 semiAxes = vec2(1.0, sqrt(1.0 - eccentricity * eccentricity)) * semiMajorAxis;   \n" +
                    "        vec2 planePos = semiAxes * vec2(cos(E) - eccentricity, sin(E));              \n" +
                    "        if (Angle.x == 0.0)                                                         \n" +
                    "           gl_Position =  matPosition * vec4(positionNow, 1.0);                                \n" +
                    "        else                                                                           \n" +
                    "           gl_Position = matWVP * vec4(planePos.x, planePos.y, 0.0, 1.0);              \n" +
                    "        vColor = vec4(color.rgb, fade * color.a);                                      \n" +
                    "    }                                                                                  \n";



            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);


            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);
            object compilationLog = gl.getShaderInfoLog(vert);
            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            AngleLoc = gl.getAttribLocation(prog, "Angle");

            matWVPLoc = gl.getUniformLocation(prog, "matWVP");
            matPositionLoc = gl.getUniformLocation(prog, "matPosition");
            positionNowLoc = gl.getUniformLocation(prog, "positionNow");
            colorLoc = gl.getUniformLocation(prog, "color");
           // opacityLoc = gl.getUniformLocation(prog, "opacity");
            semiMajorAxisLoc = gl.getUniformLocation(prog, "semiMajorAxis");
            eccentricityLoc = gl.getUniformLocation(prog, "eccentricity");
            eccentricAnomalyLoc = gl.getUniformLocation(prog, "eccentricAnomaly");
            
            gl.enable(GL.BLEND);

            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(
            RenderContext renderContext,float semiMajorAxis,float eccentricity, float eccentricAnomaly, 
            Color lineColor, float opacity, Matrix3d world, Vector3d positionNow )
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d WVPPos = Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(world,renderContext.View ),  renderContext.Projection);
                Matrix3d WVP = Matrix3d.MultiplyMatrix(Matrix3d.MultiplyMatrix(renderContext.World,renderContext.View ),  renderContext.Projection);

                gl.uniformMatrix4fv(matWVPLoc, false, WVP.FloatArray());
                gl.uniformMatrix4fv(matPositionLoc, false, WVPPos.FloatArray());
                gl.uniform3f(positionNowLoc, (float)positionNow.X, (float)positionNow.Y, (float)positionNow.Z);
                gl.uniform4f(colorLoc, lineColor.R / 255f, lineColor.G / 255f, lineColor.B / 255f, lineColor.A / 255f);
               // gl.uniform1f(opacityLoc, opacity);
                gl.uniform1f(semiMajorAxisLoc, semiMajorAxis);
                gl.uniform1f(eccentricityLoc, eccentricity);
                gl.uniform1f(eccentricAnomalyLoc, eccentricAnomaly);


              //  gl.enable(GL.DEPTH_TEST);
                gl.disable(GL.DEPTH_TEST);

                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);

                //gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                //gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);

                gl.enableVertexAttribArray(AngleLoc);
               
                gl.vertexAttribPointer(AngleLoc, 3, GL.FLOAT, false, 0, 0);
                gl.lineWidth(1.0f);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE);
            }
        }
    }

    public class ModelShader
    {
        public ModelShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int normalLoc;
        public static int textureLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation sampLoc;
        public static WebGLUniformLocation sunLoc;
        public static WebGLUniformLocation opacityLoc;
        public static WebGLUniformLocation minBrightnessLoc;
        public static WebGLUniformLocation atmosphereColorLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      " precision mediump float;                                                              \n" +
                      "                                                                                       \n" +
                      "   varying vec2 vTextureCoord;                                                         \n" +
                      "   varying vec3 vNormal;                                                               \n" +
                      "   varying vec3 vCamVector;                                                               \n" +
                      "                                                                                       \n" +
                      "   uniform sampler2D uSampler;                                                         \n" +
                      "   uniform float opacity;                                                              \n" +
                      "   uniform vec3 uSunPosition;                                                          \n" +
                      "   uniform float uMinBrightness;                                                       \n" +
                      "   uniform vec3 uAtmosphereColor;                                                       \n" +
                      "                                                                                       \n" +
                      "   void main(void) {                                                                   \n" +
                      "     vec3 normal = normalize(vNormal);                                                 \n" +
                      "     vec3 camVN = normalize(vCamVector);                                               \n" +
                      "     vec3 cam = normalize(vec3(0.0,0.0,-1.0));                                                    \n" +
                      "     float dt = uMinBrightness + pow(max(0.0,- dot(normal,uSunPosition)),0.5);                  \n" +
                      "     float atm = max(0.0, 1.0 - 2.5 * dot(cam,camVN)) + 0.3 * dt;                             \n" +
                      "     atm = (dt > uMinBrightness) ? atm : 0.0;                                          \n" +
                      "     if ( uMinBrightness == 1.0 ) { dt = 1.0; atm= 0.0; }                                        \n" +
                      "     vec4 col = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t));           \n" +
                      "     gl_FragColor = col * opacity;                                                     \n" +
                      "     gl_FragColor.rgb *= dt;                                                           \n" +
                      "     gl_FragColor.rgb += atm * uAtmosphereColor;                                  \n" + //vec3( .25, .61, .85);  
                      "   }                                                                                   \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
                    "     attribute vec3 aNormal;                                                     \n" +
                    "     attribute vec2 aTextureCoord;                                                \n" +
                    "                                                                                  \n" +
                    "     uniform mat4 uMVMatrix;                                                      \n" +
                    "     uniform mat4 uPMatrix;                                                       \n" +
                    "                                                                                  \n" +
                    "     varying vec2 vTextureCoord;                                                  \n" +
                    "     varying vec3 vNormal;                                                        \n" +
                    "     varying vec3 vCamVector;                                                     \n" +
                    "                                                                                  \n" +
                    "                                                                                  \n" +
                    "     void main(void) {                                                            \n" +
                    "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                    "         vCamVector = normalize((mat3(uMVMatrix) * aVertexPosition).xyz);              \n" +
                    "         vec3 normalT = normalize(mat3(uMVMatrix) * aNormal);                      \n" +
                    "         vTextureCoord = aTextureCoord;                                           \n" +
                    "         vNormal = normalT;                                                       \n" +
                    "     }                                                                            \n" +
                    "                                                                                  \n";

            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);
            if ((int)stat == 0)
            {
                object errorF = gl.getShaderInfoLog(frag);
            }

            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);
            if ((int)stat1 == 0)
            {
                object errorV = gl.getShaderInfoLog(vert);
            }

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            normalLoc = gl.getAttribLocation(prog, "aNormal");
            textureLoc = gl.getAttribLocation(prog, "aTextureCoord");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            sampLoc = gl.getUniformLocation(prog, "uSampler");
            sunLoc = gl.getUniformLocation(prog, "uSunPosition");
            minBrightnessLoc = gl.getUniformLocation(prog, "uMinBrightness");
            opacityLoc = gl.getUniformLocation(prog, "opacity");
            atmosphereColorLoc = gl.getUniformLocation(prog, "uAtmosphereColor");

            Tile.uvMultiple = 1;
            Tile.DemEnabled = true;

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;
        public static Vector3d SunPosition = Vector3d.Create(-1, -1, -1);
        public static float MinLightingBrightness = 1.0f;

        public static Color AtmosphereColor = Color.FromArgb(0, 0, 0, 0);
        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLBuffer index,  WebGLTexture texture, float opacity, bool noDepth, int stride)
        {
            
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);
                gl.uniform1f(opacityLoc, opacity);
                gl.uniform1f(minBrightnessLoc, renderContext.Lighting ? MinLightingBrightness : 1.0f);

                if (renderContext.Lighting)
                {
                    gl.uniform3f(atmosphereColorLoc, AtmosphereColor.R / 255.0f, AtmosphereColor.G / 255.0f, AtmosphereColor.B / 255.0f);
                }
                else
                {
                    gl.uniform3f(atmosphereColorLoc, 0f, 0f, 0f);
                }

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                SunPosition.Normalize();

                Matrix3d mvInv = renderContext.View.Clone();
                mvInv.M41 = 0;
                mvInv.M42 = 0;
                mvInv.M43 = 0;
                mvInv.M44 = 1;
                Vector3d sp = Vector3d.TransformCoordinate(SunPosition, mvInv);
                sp.Normalize();


                gl.uniform3f(sunLoc, -(float)sp.X, -(float)sp.Y, -(float)sp.Z);

                gl.uniform1i(sampLoc, 0);
                if (renderContext.Space || noDepth)
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);

                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(normalLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, stride, 0);
                gl.vertexAttribPointer(normalLoc, 3, GL.FLOAT, false, stride, 12);
                gl.vertexAttribPointer(textureLoc, 2, GL.FLOAT, false, stride, stride-8);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, index);
                gl.enable(GL.BLEND);

                if (noDepth)
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE);
                }
                else
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
                }
            }
        }
    }
  public class ModelShaderTan
    {
        public ModelShaderTan()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int normalLoc;
        public static int textureLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation sampLoc;
        public static WebGLUniformLocation sunLoc;
        public static WebGLUniformLocation opacityLoc;
        public static WebGLUniformLocation minBrightnessLoc;
        public static WebGLUniformLocation atmosphereColorLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      " precision mediump float;                                                              \n" +
                      "                                                                                       \n" +
                      "   varying vec2 vTextureCoord;                                                         \n" +
                      "   varying vec3 vNormal;                                                               \n" +
                      "   varying vec3 vCamVector;                                                               \n" +
                      "                                                                                       \n" +
                      "   uniform sampler2D uSampler;                                                         \n" +
                      "   uniform float opacity;                                                              \n" +
                      "   uniform vec3 uSunPosition;                                                          \n" +
                      "   uniform float uMinBrightness;                                                       \n" +
                      "   uniform vec3 uAtmosphereColor;                                                       \n" +
                      "                                                                                       \n" +
                      "   void main(void) {                                                                   \n" +
                      "     vec3 normal = normalize(vNormal);                                                 \n" +
                      "     vec3 camVN = normalize(vCamVector);                                               \n" +
                      "     vec3 cam = normalize(vec3(0.0,0.0,-1.0));                                                    \n" +
                      "     float dt = uMinBrightness + pow(max(0.0,- dot(normal,uSunPosition)),0.5);                  \n" +
                      "     float atm = max(0.0, 1.0 - 2.5 * dot(cam,camVN)) + 0.3 * dt;                             \n" +
                      "     atm = (dt > uMinBrightness) ? atm : 0.0;                                          \n" +
                      "     if ( uMinBrightness == 1.0 ) { dt = 1.0; atm= 0.0; }                                        \n" +
                      "     vec4 col = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t));           \n" +
                      "     gl_FragColor = col * opacity;                                                     \n" +
                      "     gl_FragColor.rgb *= dt;                                                           \n" +
                      "     gl_FragColor.rgb += atm * uAtmosphereColor;                                  \n" + //vec3( .25, .61, .85);  
                      "   }                                                                                   \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
                    "     attribute vec3 aNormal;                                                     \n" +
                    "     attribute vec2 aTextureCoord;                                                \n" +
                    "                                                                                  \n" +
                    "     uniform mat4 uMVMatrix;                                                      \n" +
                    "     uniform mat4 uPMatrix;                                                       \n" +
                    "                                                                                  \n" +
                    "     varying vec2 vTextureCoord;                                                  \n" +
                    "     varying vec3 vNormal;                                                        \n" +
                    "     varying vec3 vCamVector;                                                     \n" +
                    "                                                                                  \n" +
                    "                                                                                  \n" +
                    "     void main(void) {                                                            \n" +
                    "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                    "         vCamVector = normalize((mat3(uMVMatrix) * aVertexPosition).xyz);              \n" +
                    "         vec3 normalT = normalize(mat3(uMVMatrix) * aNormal);                      \n" +
                    "         vTextureCoord = aTextureCoord;                                           \n" +
                    "         vNormal = normalT;                                                       \n" +
                    "     }                                                                            \n" +
                    "                                                                                  \n";

            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);
            if ((int)stat == 0)
            {
                object errorF = gl.getShaderInfoLog(frag);
            }

            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);
            if ((int)stat1 == 0)
            {
                object errorV = gl.getShaderInfoLog(vert);
            }

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            normalLoc = gl.getAttribLocation(prog, "aNormal");
            textureLoc = gl.getAttribLocation(prog, "aTextureCoord");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            sampLoc = gl.getUniformLocation(prog, "uSampler");
            sunLoc = gl.getUniformLocation(prog, "uSunPosition");
            minBrightnessLoc = gl.getUniformLocation(prog, "uMinBrightness");
            opacityLoc = gl.getUniformLocation(prog, "opacity");
            atmosphereColorLoc = gl.getUniformLocation(prog, "uAtmosphereColor");

            Tile.uvMultiple = 1;
            Tile.DemEnabled = true;

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;
        public static Vector3d SunPosition = Vector3d.Create(-1, -1, -1);
        public static float MinLightingBrightness = 1.0f;

        public static Color AtmosphereColor = Color.FromArgb(0, 0, 0, 0);
        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLBuffer index,  WebGLTexture texture, float opacity, bool noDepth, int stride)
        {
            
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);
                gl.uniform1f(opacityLoc, opacity);
                gl.uniform1f(minBrightnessLoc, renderContext.Lighting ? MinLightingBrightness : 1.0f);

                if (renderContext.Lighting)
                {
                    gl.uniform3f(atmosphereColorLoc, AtmosphereColor.R / 255.0f, AtmosphereColor.G / 255.0f, AtmosphereColor.B / 255.0f);
                }
                else
                {
                    gl.uniform3f(atmosphereColorLoc, 0f, 0f, 0f);
                }

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                SunPosition.Normalize();

                Matrix3d mvInv = renderContext.View.Clone();
                mvInv.M41 = 0;
                mvInv.M42 = 0;
                mvInv.M43 = 0;
                mvInv.M44 = 1;
                Vector3d sp = Vector3d.TransformCoordinate(SunPosition, mvInv);
                sp.Normalize();


                gl.uniform3f(sunLoc, -(float)sp.X, -(float)sp.Y, -(float)sp.Z);

                gl.uniform1i(sampLoc, 0);
                if (renderContext.Space || noDepth)
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);

                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(normalLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, stride, 0);
                gl.vertexAttribPointer(normalLoc, 3, GL.FLOAT, false, stride, 12);
                gl.vertexAttribPointer(textureLoc, 2, GL.FLOAT, false, stride, stride-8);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, index);
                gl.enable(GL.BLEND);

                if (noDepth)
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE);
                }
                else
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
                }
            }
        }
    }


    public class TileShader
    {
        public TileShader()
        {

        }
        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int textureLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation sampLoc;
        public static WebGLUniformLocation sunLoc;
        public static WebGLUniformLocation opacityLoc;
        public static WebGLUniformLocation minBrightnessLoc;
        public static WebGLUniformLocation atmosphereColorLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      " precision mediump float;                                                              \n" +
                      "                                                                                       \n" +
                      "   varying vec2 vTextureCoord;                                                         \n" +
                      "   varying vec3 vNormal;                                                               \n" +
                      "   varying vec3 vCamVector;                                                               \n" +
                      "                                                                                       \n" +
                      "   uniform sampler2D uSampler;                                                         \n" +
                      "   uniform float opacity;                                                              \n" +
                      "   uniform vec3 uSunPosition;                                                          \n" +
                      "   uniform float uMinBrightness;                                                       \n" +
                      "   uniform vec3 uAtmosphereColor;                                                       \n" +
                      "                                                                                       \n" +
                      "   void main(void) {                                                                   \n" +
                      "     vec3 normal = normalize(vNormal);                                                 \n" +
                      "     vec3 camVN = normalize(vCamVector);                                               \n" +
                      "     vec3 cam = normalize(vec3(0.0,0.0,-1.0));                                                    \n" +
                      "     float dt = uMinBrightness + pow(max(0.0,- dot(normal,uSunPosition)),0.5);                  \n" +
                      "     float atm = max(0.0, 1.0 - 2.5 * dot(cam,camVN)) + 0.3 * dt;                             \n" +
                      "     atm = (dt > uMinBrightness) ? atm : 0.0;                                          \n" +
                      "     if ( uMinBrightness == 1.0 ) { dt = 1.0; atm= 0.0; }                                        \n" +
                      "     vec4 col = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t));           \n" +
                      "     gl_FragColor = col * opacity;                                                     \n" +
                      "     gl_FragColor.rgb *= dt;                                                           \n" +
                      "     gl_FragColor.rgb += atm * uAtmosphereColor;                                  \n" + //vec3( .25, .61, .85);  
                      "   }                                                                                   \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
                    "     attribute vec2 aTextureCoord;                                                \n" +
                    "                                                                                  \n" +
                    "     uniform mat4 uMVMatrix;                                                      \n" +
                    "     uniform mat4 uPMatrix;                                                       \n" +
                    "                                                                                  \n" +
                    "     varying vec2 vTextureCoord;                                                  \n" +
                    "     varying vec3 vNormal;                                                        \n" +
                    "     varying vec3 vCamVector;                                                     \n" +
                    "                                                                                  \n" +
                    "                                                                                  \n" +
                    "     void main(void) {                                                            \n" +
                    "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                    "         vCamVector = normalize((mat3(uMVMatrix) * aVertexPosition).xyz);              \n" +
                    "         vec3 normal = normalize(aVertexPosition);                                \n" +
                    "         vec3 normalT = normalize(mat3(uMVMatrix) * normal);                      \n" +
                    "         vTextureCoord = aTextureCoord;                                           \n" +
                    "         vNormal = normalT;                                                       \n" +
                    "     }                                                                            \n" +
                    "                                                                                  \n";

            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);
            if ((int)stat == 0)
            {
                object errorF = gl.getShaderInfoLog(frag);
            }

            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);
            if ((int)stat1 == 0)
            {
                object errorV = gl.getShaderInfoLog(vert);
            }

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            textureLoc = gl.getAttribLocation(prog, "aTextureCoord");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            sampLoc = gl.getUniformLocation(prog, "uSampler");
            sunLoc = gl.getUniformLocation(prog, "uSunPosition");
            minBrightnessLoc = gl.getUniformLocation(prog, "uMinBrightness");
            opacityLoc = gl.getUniformLocation(prog, "opacity");
            atmosphereColorLoc = gl.getUniformLocation(prog, "uAtmosphereColor");

            Tile.uvMultiple = 1;
            Tile.DemEnabled = true;

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;
        public static Vector3d SunPosition = Vector3d.Create(-1, -1, -1);
        public static float MinLightingBrightness = 1.0f;

        public static Color AtmosphereColor = Color.FromArgb(0, 0, 0, 0);
        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLBuffer index, WebGLTexture texture, float opacity, bool noDepth)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);
                gl.uniform1f(opacityLoc, opacity);
                gl.uniform1f(minBrightnessLoc, renderContext.Lighting ? MinLightingBrightness : 1.0f);

                if (renderContext.Lighting)
                {
                    gl.uniform3f(atmosphereColorLoc, AtmosphereColor.R / 255.0f, AtmosphereColor.G / 255.0f, AtmosphereColor.B / 255.0f);
                }
                else
                {
                    gl.uniform3f(atmosphereColorLoc, 0f, 0f, 0f);
                }

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                SunPosition.Normalize();

                Matrix3d mvInv = renderContext.View.Clone();
                mvInv.M41 = 0;
                mvInv.M42 = 0;
                mvInv.M43 = 0;
                mvInv.M44 = 1;
                Vector3d sp = Vector3d.TransformCoordinate(SunPosition, mvInv);
                sp.Normalize();


                gl.uniform3f(sunLoc, -(float)sp.X, -(float)sp.Y, -(float)sp.Z);

                gl.uniform1i(sampLoc, 0);
                if (renderContext.Space || noDepth)
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);

                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 20, 0);
                gl.vertexAttribPointer(textureLoc, 2, GL.FLOAT, false, 20, 12);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, index);
                gl.enable(GL.BLEND);

                if (noDepth)
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE);
                }
                else
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
                }
            }
        }
    }

    public class ImageShader
    {
        public ImageShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int textureLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation sampLoc;
        public static WebGLUniformLocation opacityLoc;


        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      " precision mediump float;                                                              \n" +
                      "                                                                                       \n" +
                      "   varying vec2 vTextureCoord;                                                         \n" +
                      "                                                                                       \n" +
                      "   uniform sampler2D uSampler;                                                         \n" +
                      "   uniform float opacity;                                                              \n" +

                      "                                                                                       \n" +
                      "   void main(void) {                                                                   \n" +
                      "     vec4 col = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t));           \n" +
                      "     gl_FragColor = col * opacity;                                                     \n" +
                      "   }                                                                                   \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
                    "     attribute vec2 aTextureCoord;                                                \n" +
                    "                                                                                  \n" +
                    "     uniform mat4 uMVMatrix;                                                      \n" +
                    "     uniform mat4 uPMatrix;                                                       \n" +
                    "                                                                                  \n" +
                    "     varying vec2 vTextureCoord;                                                  \n" +
                    "     varying vec3 vNormal;                                                        \n" +
                    "     varying vec3 vCamVector;                                                     \n" +
                    "                                                                                  \n" +
                    "                                                                                  \n" +
                    "     void main(void) {                                                            \n" +
                    "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                    "         vTextureCoord = aTextureCoord;                                           \n" +
                    "     }                                                                            \n" +
                    "                                                                                  \n";

            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);
            if ((int)stat == 0)
            {
                object errorF = gl.getShaderInfoLog(frag);
            }

            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);
            if ((int)stat1 == 0)
            {
                object errorV = gl.getShaderInfoLog(vert);
            }

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            textureLoc = gl.getAttribLocation(prog, "aTextureCoord");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            sampLoc = gl.getUniformLocation(prog, "uSampler");
            opacityLoc = gl.getUniformLocation(prog, "opacity");

            Tile.uvMultiple = 1;
            Tile.DemEnabled = true;

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLBuffer index, WebGLTexture texture, float opacity, bool noDepth)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);
                gl.uniform1f(opacityLoc, opacity);


                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());

                gl.uniform1i(sampLoc, 0);
                if (renderContext.Space || noDepth)
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);

                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 20, 0);
                gl.vertexAttribPointer(textureLoc, 2, GL.FLOAT, false, 20, 12);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, index);
                gl.enable(GL.BLEND);

                if (noDepth)
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE);
                }
                else
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
                }
            }
        }
    }

    public class ImageShader2
    {
        public ImageShader2()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int textureLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation sampLoc;
        public static WebGLUniformLocation opacityLoc;


        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      " precision mediump float;                                                              \n" +
                      "                                                                                       \n" +
                      "   varying vec2 vTextureCoord;                                                         \n" +
                      "                                                                                       \n" +
                      "   uniform sampler2D uSampler;                                                         \n" +
                      "   uniform float opacity;                                                              \n" +

                      "                                                                                       \n" +
                      "   void main(void) {                                                                   \n" +
                      "     vec4 col = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t));           \n" +
                      "     gl_FragColor = col * opacity;                                                     \n" +
                      "   }                                                                                   \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
                    "     attribute vec2 aTextureCoord;                                                \n" +
                    "                                                                                  \n" +
                    "     uniform mat4 uMVMatrix;                                                      \n" +
                    "     uniform mat4 uPMatrix;                                                       \n" +
                    "                                                                                  \n" +
                    "     varying vec2 vTextureCoord;                                                  \n" +
                    "     varying vec3 vNormal;                                                        \n" +
                    "     varying vec3 vCamVector;                                                     \n" +
                    "                                                                                  \n" +
                    "                                                                                  \n" +
                    "     void main(void) {                                                            \n" +
                    "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                    "         vTextureCoord = aTextureCoord;                                           \n" +
                    "     }                                                                            \n" +
                    "                                                                                  \n";

            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);
            if ((int)stat == 0)
            {
                object errorF = gl.getShaderInfoLog(frag);
            }

            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);
            if ((int)stat1 == 0)
            {
                object errorV = gl.getShaderInfoLog(vert);
            }

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            textureLoc = gl.getAttribLocation(prog, "aTextureCoord");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            sampLoc = gl.getUniformLocation(prog, "uSampler");
            opacityLoc = gl.getUniformLocation(prog, "opacity");

            Tile.uvMultiple = 1;
            Tile.DemEnabled = true;

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLBuffer index, WebGLTexture texture, float opacity, bool noDepth)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);
                gl.uniform1f(opacityLoc, opacity);


                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());

                gl.uniform1i(sampLoc, 0);
                if (renderContext.Space || noDepth)
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.enable(GL.DEPTH_TEST);
                }
                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);

                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 32, 0);
                gl.vertexAttribPointer(textureLoc, 2, GL.FLOAT, false, 32, 24);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, index);
                gl.enable(GL.BLEND);

                if (noDepth)
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE);
                }
                else
                {
                    gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
                }
            }
        }
    }


    public class SpriteShader
    {
        public SpriteShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int textureLoc;
        public static int colorLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation sampLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      " precision mediump float;                                                                \n" +
                      "                                                                                         \n" +
                      "   varying vec2 vTextureCoord;                                                           \n" +
                      "   varying lowp vec4 vColor;                                                             \n" +
                      "   uniform sampler2D uSampler;                                                           \n" +
                      "                                                                                         \n" +
                      "   void main(void) {                                                                     \n" +
                      "   gl_FragColor = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t)) * vColor;  \n" +
                      "   }                                                                                     \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
                    "     attribute vec2 aTextureCoord;                                                \n" +
                    "     attribute lowp vec4 aColor;                                                  \n" +
                    "                                                                                  \n" +
                    "     uniform mat4 uMVMatrix;                                                      \n" +
                    "     uniform mat4 uPMatrix;                                                       \n" +
                    "                                                                                  \n" +
                    "     varying vec2 vTextureCoord;                                                  \n" +
                    "     varying vec4 vColor;                                                         \n" +
                    "                                                                                  \n" +
                    "                                                                                  \n" +
                    "     void main(void) {                                                            \n" +
                    "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                    "         vTextureCoord = aTextureCoord;                                           \n" +
                    "         vColor = aColor;                                                         \n" +
                    "     }                                                                            \n" +
                    "                                                                                  \n";

            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);


            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            textureLoc = gl.getAttribLocation(prog, "aTextureCoord");
            colorLoc = gl.getAttribLocation(prog, "aColor");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            sampLoc = gl.getUniformLocation(prog, "uSampler");

            Tile.uvMultiple = 1;
            Tile.DemEnabled = true;

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLTexture texture)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                gl.uniform1i(sampLoc, 0);

                gl.disable(GL.DEPTH_TEST);

                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.enableVertexAttribArray(colorLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 36, 0);
                gl.vertexAttribPointer(colorLoc, 4, GL.FLOAT, false, 36, 12);
                gl.vertexAttribPointer(textureLoc, 2, GL.FLOAT, false, 36, 28);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            }
        }
    }

    public class ShapeSpriteShader
    {
        public ShapeSpriteShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int textureLoc;
        public static int colorLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation sampLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      " precision mediump float;                                                                \n" +
                      "                                                                                         \n" +
                      "   varying lowp vec4 vColor;                                                             \n" +
                      "                                                                                         \n" +
                      "   void main(void) {                                                                     \n" +
                      "   gl_FragColor =  vColor;                                                               \n" +
                      "   }                                                                                     \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
                    "     attribute lowp vec4 aColor;                                                  \n" +
                    "                                                                                  \n" +
                    "     uniform mat4 uMVMatrix;                                                      \n" +
                    "     uniform mat4 uPMatrix;                                                       \n" +
                    "                                                                                  \n" +
                    "     varying vec2 vTextureCoord;                                                  \n" +
                    "     varying vec4 vColor;                                                         \n" +
                    "                                                                                  \n" +
                    "                                                                                  \n" +
                    "     void main(void) {                                                            \n" +
                    "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                    "         vColor = aColor;                                                         \n" +
                    "     }                                                                            \n" +
                    "                                                                                  \n";

            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);


            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            colorLoc = gl.getAttribLocation(prog, "aColor");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");

            gl.disable(GL.DEPTH_TEST);
            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                gl.uniform1i(sampLoc, 0);

                gl.disable(GL.DEPTH_TEST);

                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);
                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
             
                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.enableVertexAttribArray(colorLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 36, 0);
                gl.vertexAttribPointer(colorLoc, 4, GL.FLOAT, false, 36, 12);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            }
        }
    }

    public class TextShader
    {
        public TextShader()
        {

        }

        internal static WebGLShader frag;
        internal static WebGLShader vert;


        public static int vertLoc;
        public static int textureLoc;
        public static WebGLUniformLocation projMatLoc;
        public static WebGLUniformLocation mvMatLoc;
        public static WebGLUniformLocation sampLoc;

        public static bool initialized = false;
        public static void Init(RenderContext renderContext)
        {
            GL gl = renderContext.gl;

            String fragShaderText =
                      " precision mediump float;                                                              \n" +
                      "                                                                                       \n" +
                      "   varying vec2 vTextureCoord;                                                         \n" +
                      "                                                                                       \n" +
                      "   uniform sampler2D uSampler;                                                         \n" +
                      "                                                                                       \n" +
                      "   void main(void) {                                                                   \n" +
                      "   gl_FragColor = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t));         \n" +
                      "   }                                                                                   \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
                    "     attribute vec2 aTextureCoord;                                                \n" +
                    "                                                                                  \n" +
                    "     uniform mat4 uMVMatrix;                                                      \n" +
                    "     uniform mat4 uPMatrix;                                                       \n" +
                    "                                                                                  \n" +
                    "     varying vec2 vTextureCoord;                                                  \n" +
                    "                                                                                  \n" +
                    "                                                                                  \n" +
                    "     void main(void) {                                                            \n" +
                    "         gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);         \n" +
                    "         vTextureCoord = aTextureCoord;                                           \n" +
                    "     }                                                                            \n" +
                    "                                                                                  \n";
            frag = gl.createShader(GL.FRAGMENT_SHADER);
            gl.shaderSource(frag, fragShaderText);
            gl.compileShader(frag);

            object stat = gl.getShaderParameter(frag, GL.COMPILE_STATUS);


            vert = gl.createShader(GL.VERTEX_SHADER);
            gl.shaderSource(vert, vertexShaderText);
            gl.compileShader(vert);
            object stat1 = gl.getShaderParameter(vert, GL.COMPILE_STATUS);

            prog = gl.createProgram();

            gl.attachShader(prog, vert);
            gl.attachShader(prog, frag);
            gl.linkProgram(prog);
            object errcode = gl.getProgramParameter(prog, GL.LINK_STATUS);


            gl.useProgram(prog);

            vertLoc = gl.getAttribLocation(prog, "aVertexPosition");
            textureLoc = gl.getAttribLocation(prog, "aTextureCoord");
            projMatLoc = gl.getUniformLocation(prog, "uPMatrix");
            mvMatLoc = gl.getUniformLocation(prog, "uMVMatrix");
            sampLoc = gl.getUniformLocation(prog, "uSampler");

            Tile.uvMultiple = 1;
            Tile.DemEnabled = true;

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;

        //todo add color rendering
        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLTexture texture)
        {
            GL gl = renderContext.gl;
            if (gl != null)
            {
                if (!initialized)
                {
                    Init(renderContext);
                }

                gl.useProgram(prog);

                Matrix3d mvMat = Matrix3d.MultiplyMatrix(renderContext.World, renderContext.View);

                gl.uniformMatrix4fv(mvMatLoc, false, mvMat.FloatArray());
                gl.uniformMatrix4fv(projMatLoc, false, renderContext.Projection.FloatArray());
                gl.uniform1i(sampLoc, 0);
                if (renderContext.Space)
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.enable(GL.DEPTH_TEST);
                }



                gl.disableVertexAttribArray(0);
                gl.disableVertexAttribArray(1);
                gl.disableVertexAttribArray(2);
                gl.disableVertexAttribArray(3);
                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);

                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 20, 0);
                gl.vertexAttribPointer(textureLoc, 2, GL.FLOAT, false, 20, 12);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            }
        }
    }

}
