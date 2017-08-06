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
                      " precision highp float;                                                              \n" +
                      " uniform vec4 lineColor;                                                               \n" +
                      "                                                                                       \n" +
                      "   void main(void) {                                                                   \n" +
                      "   gl_FragColor = lineColor;         \n" +
                      "   }                                                                                   \n";


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
                gl.uniform4f(lineColorLoc, lineColor.R/255, lineColor.G/255, lineColor.B/255, 1);
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
                    "     if (jNow < aTime.x && decay > 0.0)                                                 \n" +
                    "     {                                                                                  \n" +
                    //"         vColor = vec4(0.0, 0.0, 0.0, 0.0);                                             \n" +
                    "         vColor = vec4(1, 1, 1, 1);                                                    \n" +
                    "     }                                                                                  \n" +
                    "     else                                                                               \n" +
                    "     {                                                                                  \n" +
                    "        vColor = vec4(aVertexColor.r, aVertexColor.g, aVertexColor.b, dAlpha * aVertexColor.a);          \n" +
                    //"         vColor = vec4(1, 1, 1, 1);                                                    \n" +

                    "     }                                                                                  \n" +
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


                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(colorLoc);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
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
                    "                                                                                        \n" +
                    "    varying lowp vec4 vColor;                                                           \n" +
                    "                                                                                        \n" +
                    "    void main(void)                                                                     \n" +
                    "    {                                                                                   \n" +
                    "        float dist = distance(aVertexPosition, cameraPosition);                                \n" +
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
                    "            vColor = vec4(0.0, 0.0, 0.0, 0.0);                                          \n" +
                    "        }                                                                               \n" +
                    "        else                                                                            \n" +
                    "        {                                                                               \n" +
                    "           vColor = vec4(aVertexColor.r, aVertexColor.g, aVertexColor.b, dAlpha);       \n" +
                   // "           vColor = vec4(1,1,1,1);       \n" +

                    "        }                                                                               \n" +
                    "        gl_PointSize = max(2.0, (scale * ( aPointSize ) / dist));                     \n" +
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
            gl.enable(GL.BLEND);
           
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLTexture texture, Color lineColor, bool zBuffer, float jNow, float decay, Vector3d camera, float scale)
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
                gl.uniform4f(lineColorLoc, lineColor.R / 255f, lineColor.G / 255f, lineColor.B / 255f, lineColor.A/255f);
                gl.uniform3f(cameraPosLoc, (float)camera.X, (float)camera.Y, (float)camera.Z);
                gl.uniform1f(scaleLoc, scale );
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


                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(colorLoc);
                gl.enableVertexAttribArray(pointSizeLoc);
                gl.enableVertexAttribArray(timeLoc);

                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
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
                      "   vec4 col = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t));             \n" +
                      "   gl_FragColor = col * opacity;                                                       \n" +
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
            opacityLoc = gl.getUniformLocation(prog, "opacity");

            Tile.uvMultiple = 1;
            Tile.DemEnabled = true;

            gl.enable(GL.BLEND);
            gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            initialized = true;
        }

        private static WebGLProgram prog = null;

        public static void Use(RenderContext renderContext, WebGLBuffer vertex, WebGLBuffer index, WebGLTexture texture, float opacity)
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
                if (renderContext.Space)
                {
                    gl.disable(GL.DEPTH_TEST);
                }
                else
                {
                    gl.enable(GL.DEPTH_TEST);
                }

                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 20, 0);
                gl.vertexAttribPointer(textureLoc, 2, GL.FLOAT, false, 20, 12);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, index);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
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
 //                     "   gl_FragColor = vec4(1,1,1,1);  \n" +
                      "   }                                                                                     \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
                    "     attribute vec2 aTextureCoord;                                                \n" +
                    "     attribute lowp vec4 aColor;                                                \n" +
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
               
                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.enableVertexAttribArray(colorLoc);
                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
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
                      "   gl_FragColor =  vColor;  \n" +
                      //                     "   gl_FragColor = vec4(1,1,1,1);  \n" +
                      "   }                                                                                     \n";


            String vertexShaderText =
                    "     attribute vec3 aVertexPosition;                                              \n" +
               //     "     attribute vec2 aTextureCoord;                                                \n" +
                    "     attribute lowp vec4 aColor;                                                \n" +
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
                 //   "         vTextureCoord = aTextureCoord;                                           \n" +
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
             
                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.enableVertexAttribArray(colorLoc);
                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
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
                //gl.disableVertexAttribArray(0);
                //gl.disableVertexAttribArray(1);
                //gl.disableVertexAttribArray(2);
                //gl.disableVertexAttribArray(3);

                gl.enableVertexAttribArray(vertLoc);
                gl.enableVertexAttribArray(textureLoc);
                gl.bindBuffer(GL.ARRAY_BUFFER, vertex);
                gl.vertexAttribPointer(vertLoc, 3, GL.FLOAT, false, 20, 0);
                gl.vertexAttribPointer(textureLoc, 2, GL.FLOAT, false, 20, 12);
                gl.activeTexture(GL.TEXTURE0);
                gl.bindTexture(GL.TEXTURE_2D, texture);
                //gl.bindBuffer(GL.ELEMENT_ARRAY_BUFFER, null);
                gl.enable(GL.BLEND);
                gl.blendFunc(GL.SRC_ALPHA, GL.ONE_MINUS_SRC_ALPHA);
            }
        }
    }

}
