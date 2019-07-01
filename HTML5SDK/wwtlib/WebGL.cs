using System;
using System.Runtime.CompilerServices;
using System.Html.Media.Graphics;

namespace System.Html
{
    [ScriptIgnoreNamespace]
    [ScriptImport]
    // [ScriptName("globalCompositeOperation")]
    public class GL
    {
        public const int ACTIVE_ATTRIBUTE_MAX_LENGTH = 35722;
        public const int ACTIVE_ATTRIBUTES = 35721;
        public const int ACTIVE_TEXTURE = 34016;
        public const int ACTIVE_UNIFORM_MAX_LENGTH = 35719;
        public const int ACTIVE_UNIFORMS = 35718;
        public const int ALIASED_LINE_WIDTH_RANGE = 33902;
        public const int ALIASED_POINT_SIZE_RANGE = 33901;
        public const int ALPHA = 6406;
        public const int ALPHA_BITS = 3413;
        public const int ALWAYS = 519;
        public const int ARRAY_BUFFER = 34962;
        public const int ARRAY_BUFFER_BINDING = 34964;
        public const int ATTACHED_SHADERS = 35717;
        public const int BACK = 1029;
        public const int BLEND = 3042;
        public const int BLEND_COLOR = 32773;
        public const int BLEND_DST_ALPHA = 32970;
        public const int BLEND_DST_RGB = 32968;
        public const int BLEND_EQUATION = 32777;
        public const int BLEND_EQUATION_ALPHA = 34877;
        public const int BLEND_EQUATION_RGB = 32777;
        public const int BLEND_SRC_ALPHA = 32971;
        public const int BLEND_SRC_RGB = 32969;
        public const int BLUE_BITS = 3412;
        public const int BOOL = 35670;
        public const int BOOL_VEC2 = 35671;
        public const int BOOL_VEC3 = 35672;
        public const int BOOL_VEC4 = 35673;
        public const int BUFFER_SIZE = 34660;
        public const int BUFFER_USAGE = 34661;
        public const int BYTE = 5120;
        public const int CCW = 2305;
        public const int CLAMP_TO_EDGE = 33071;
        public const int COLOR_ATTACHMENT0 = 36064;
        public const int COLOR_BUFFER_BIT = 16384;
        public const int COLOR_CLEAR_VALUE = 3106;
        public const int COLOR_WRITEMASK = 3107;
        public const int COMPILE_STATUS = 35713;
        public const int COMPRESSED_TEXTURE_FORMATS = 34467;
        public const int CONSTANT_ALPHA = 32771;
        public const int CONSTANT_COLOR = 32769;
        public const int CULL_FACE = 2884;
        public const int CULL_FACE_MODE = 2885;
        public const int CURRENT_PROGRAM = 35725;
        public const int CURRENT_VERTEX_ATTRIB = 34342;
        public const int CW = 2304;
        public const int DECR = 7683;
        public const int DECR_WRAP = 34056;
        public const int DELETE_STATUS = 35712;
        public const int DEPTH_ATTACHMENT = 36096;
        public const int DEPTH_BITS = 3414;
        public const int DEPTH_BUFFER_BIT = 256;
        public const int DEPTH_CLEAR_VALUE = 2931;
        public const int DEPTH_COMPONENT = 6402;
        public const int DEPTH_COMPONENT16 = 33189;
        public const int DEPTH_FUNC = 2932;
        public const int DEPTH_RANGE = 2928;
        public const int DEPTH_STENCIL = 34041;
        public const int DEPTH_STENCIL_ATTACHMENT = 33306;
        public const int DEPTH_TEST = 2929;
        public const int DEPTH_WRITEMASK = 2930;
        public const int DITHER = 3024;
        public const int DONT_CARE = 4352;
        public const int DST_ALPHA = 772;
        public const int DST_COLOR = 774;
        public const int DYNAMIC_DRAW = 35048;
        public const int ELEMENT_ARRAY_BUFFER = 34963;
        public const int ELEMENT_ARRAY_BUFFER_BINDING = 34965;
        public const int EQUAL = 514;
        public const int EXTENSIONS = 7939;
        public const int FASTEST = 4353;
        public const int FLOAT = 5126;
        public const int FLOAT_MAT2 = 35674;
        public const int FLOAT_MAT3 = 35675;
        public const int FLOAT_MAT4 = 35676;
        public const int FLOAT_VEC2 = 35664;
        public const int FLOAT_VEC3 = 35665;
        public const int FLOAT_VEC4 = 35666;
        public const int FRAGMENT_SHADER = 35632;
        public const int FRAMEBUFFER = 36160;
        public const int FRAMEBUFFER_ATTACHMENT_OBJECT_NAME = 36049;
        public const int FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE = 36048;
        public const int FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE = 36051;
        public const int FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL = 36050;
        public const int FRAMEBUFFER_BINDING = 36006;
        public const int FRAMEBUFFER_COMPLETE = 36053;
        public const int FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 36054;
        public const int FRAMEBUFFER_INCOMPLETE_DIMENSIONS = 36057;
        public const int FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 36055;
        public const int FRAMEBUFFER_UNSUPPORTED = 36061;
        public const int FRONT = 1028;
        public const int FRONT_AND_BACK = 1032;
        public const int FRONT_FACE = 2886;
        public const int FUNC_ADD = 32774;
        public const int FUNC_REVERSE_SUBTRACT = 32779;
        public const int FUNC_SUBTRACT = 32778;
        public const int GENERATE_MIPMAP_HINT = 33170;
        public const int GEQUAL = 518;
        public const int GREATER = 516;
        public const int GREEN_BITS = 3411;
        public const int HIGH_FLOAT = 36338;
        public const int HIGH_INT = 36341;
        public const int IMPLEMENTATION_COLOR_READ_FORMAT = 35739;
        public const int IMPLEMENTATION_COLOR_READ_TYPE = 35738;
        public const int INCR = 7682;
        public const int INCR_WRAP = 34055;
        public const int INFO_LOG_LENGTH = 35716;
        public const int INT = 5124;
        public const int INT_VEC2 = 35667;
        public const int INT_VEC3 = 35668;
        public const int INT_VEC4 = 35669;
        public const int INVALID_ENUM = 1280;
        public const int INVALID_FRAMEBUFFER_OPERATION = 1286;
        public const int INVALID_OPERATION = 1282;
        public const int INVALID_VALUE = 1281;
        public const int INVERT = 5386;
        public const int KEEP = 7680;
        public const int LEQUAL = 515;
        public const int LESS = 513;
        public const int LINE_LOOP = 2;
        public const int LINE_STRIP = 3;
        public const int LINE_WIDTH = 2849;
        public const int LINEAR = 9729;
        public const int LINEAR_MIPMAP_LINEAR = 9987;
        public const int LINEAR_MIPMAP_NEAREST = 9985;
        public const int LINES = 1;
        public const int LINK_STATUS = 35714;
        public const int LOW_FLOAT = 36336;
        public const int LOW_INT = 36339;
        public const int LUMINANCE = 6409;
        public const int LUMINANCE_ALPHA = 6410;
        public const int MAX_COMBINED_TEXTURE_IMAGE_UNITS = 35661;
        public const int MAX_CUBE_MAP_TEXTURE_SIZE = 34076;
        public const int MAX_FRAGMENT_UNIFORM_VECTORS = 36349;
        public const int MAX_RENDERBUFFER_SIZE = 34024;
        public const int MAX_TEXTURE_IMAGE_UNITS = 34930;
        public const int MAX_TEXTURE_SIZE = 3379;
        public const int MAX_VARYING_VECTORS = 36348;
        public const int MAX_VERTEX_ATTRIBS = 34921;
        public const int MAX_VERTEX_TEXTURE_IMAGE_UNITS = 35660;
        public const int MAX_VERTEX_UNIFORM_VECTORS = 36347;
        public const int MAX_VIEWPORT_DIMS = 3386;
        public const int MEDIUM_FLOAT = 36337;
        public const int MEDIUM_INT = 36340;
        public const int MIRRORED_REPEAT = 33648;
        public const int NEAREST = 9728;
        public const int NEAREST_MIPMAP_LINEAR = 9986;
        public const int NEAREST_MIPMAP_NEAREST = 9984;
        public const int NEVER = 512;
        public const int NICEST = 4354;
        public const int NO_ERROR = 0;
        public const int NONE = 0;
        public const int NOTEQUAL = 517;
        public const int NUM_COMPRESSED_TEXTURE_FORMATS = 34466;
        public const int ONE = 1;
        public const int ONE_MINUS_CONSTANT_ALPHA = 32772;
        public const int ONE_MINUS_CONSTANT_COLOR = 32770;
        public const int ONE_MINUS_DST_ALPHA = 773;
        public const int ONE_MINUS_DST_COLOR = 775;
        public const int ONE_MINUS_SRC_ALPHA = 771;
        public const int ONE_MINUS_SRC_COLOR = 769;
        public const int OUT_OF_MEMORY = 1285;
        public const int PACK_ALIGNMENT = 3333;
        public const int POINTS = 0;
        public const int POLYGON_OFFSET_FACTOR = 32824;
        public const int POLYGON_OFFSET_FILL = 32823;
        public const int POLYGON_OFFSET_UNITS = 10752;
        public const int RED_BITS = 3410;
        public const int RENDERBUFFER = 36161;
        public const int RENDERBUFFER_ALPHA_SIZE = 36179;
        public const int RENDERBUFFER_BINDING = 36007;
        public const int RENDERBUFFER_BLUE_SIZE = 36178;
        public const int RENDERBUFFER_DEPTH_SIZE = 36180;
        public const int RENDERBUFFER_GREEN_SIZE = 36177;
        public const int RENDERBUFFER_HEIGHT = 36163;
        public const int RENDERBUFFER_INTERNAL_FORMAT = 36164;
        public const int RENDERBUFFER_RED_SIZE = 36176;
        public const int RENDERBUFFER_STENCIL_SIZE = 36181;
        public const int RENDERBUFFER_WIDTH = 36162;
        public const int RENDERER = 7937;
        public const int REPEAT = 10497;
        public const int REPLACE = 7681;
        public const int RGB = 6407;
        public const int RGB5_A1 = 32855;
        public const int RGB565 = 36194;
        public const int RGBA = 6408;
        public const int RGBA4 = 32854;
        public const int SAMPLE_ALPHA_TO_COVERAGE = 32926;
        public const int SAMPLE_BUFFERS = 32936;
        public const int SAMPLE_COVERAGE = 32928;
        public const int SAMPLE_COVERAGE_INVERT = 32939;
        public const int SAMPLE_COVERAGE_VALUE = 32938;
        public const int SAMPLER_2D = 35678;
        public const int SAMPLER_CUBE = 35680;
        public const int SAMPLES = 32937;
        public const int SCISSOR_BOX = 3088;
        public const int SCISSOR_TEST = 3089;
        public const int SHADER_COMPILER = 36346;
        public const int SHADER_SOURCE_LENGTH = 35720;
        public const int SHADER_TYPE = 35663;
        public const int SHADING_LANGUAGE_VERSION = 35724;
        public const int SHORT = 5122;
        public const int SRC_ALPHA = 770;
        public const int SRC_ALPHA_SATURATE = 776;
        public const int SRC_COLOR = 768;
        public const int STATIC_DRAW = 35044;
        public const int STENCIL_ATTACHMENT = 36128;
        public const int STENCIL_BACK_FAIL = 34817;
        public const int STENCIL_BACK_FUNC = 34816;
        public const int STENCIL_BACK_PASS_DEPTH_FAIL = 34818;
        public const int STENCIL_BACK_PASS_DEPTH_PASS = 34819;
        public const int STENCIL_BACK_REF = 36003;
        public const int STENCIL_BACK_VALUE_MASK = 36004;
        public const int STENCIL_BACK_WRITEMASK = 36005;
        public const int STENCIL_BITS = 3415;
        public const int STENCIL_BUFFER_BIT = 1024;
        public const int STENCIL_CLEAR_VALUE = 2961;
        public const int STENCIL_FAIL = 2964;
        public const int STENCIL_FUNC = 2962;
        public const int STENCIL_INDEX = 6401;
        public const int STENCIL_INDEX8 = 36168;
        public const int STENCIL_PASS_DEPTH_FAIL = 2965;
        public const int STENCIL_PASS_DEPTH_PASS = 2966;
        public const int STENCIL_REF = 2967;
        public const int STENCIL_TEST = 2960;
        public const int STENCIL_VALUE_MASK = 2963;
        public const int STENCIL_WRITEMASK = 2968;
        public const int STREAM_DRAW = 35040;
        public const int SUBPIXEL_BITS = 3408;
        public const int TEXTURE = 5890;
        public const int TEXTURE_2D = 3553;
        public const int TEXTURE_BINDING_2D = 32873;
        public const int TEXTURE_BINDING_CUBE_MAP = 34068;
        public const int TEXTURE_CUBE_MAP = 34067;
        public const int TEXTURE_CUBE_MAP_NEGATIVE_X = 34070;
        public const int TEXTURE_CUBE_MAP_NEGATIVE_Y = 34072;
        public const int TEXTURE_CUBE_MAP_NEGATIVE_Z = 34074;
        public const int TEXTURE_CUBE_MAP_POSITIVE_X = 34069;
        public const int TEXTURE_CUBE_MAP_POSITIVE_Y = 34071;
        public const int TEXTURE_CUBE_MAP_POSITIVE_Z = 34073;
        public const int UNPACK_FLIP_Y_WEBGL = 37440;
        public const int TEXTURE_MAG_FILTER = 10240;
        public const int TEXTURE_MIN_FILTER = 10241;
        public const int TEXTURE_WRAP_S = 10242;
        public const int TEXTURE_WRAP_T = 10243;
        public const int TEXTURE0 = 33984;
        public const int TEXTURE1 = 33985;
        public const int TEXTURE10 = 33994;
        public const int TEXTURE11 = 33995;
        public const int TEXTURE12 = 33996;
        public const int TEXTURE13 = 33997;
        public const int TEXTURE14 = 33998;
        public const int TEXTURE15 = 33999;
        public const int TEXTURE16 = 34000;
        public const int TEXTURE17 = 34001;
        public const int TEXTURE18 = 34002;
        public const int TEXTURE19 = 34003;
        public const int TEXTURE2 = 33986;
        public const int TEXTURE20 = 34004;
        public const int TEXTURE21 = 34005;
        public const int TEXTURE22 = 34006;
        public const int TEXTURE23 = 34007;
        public const int TEXTURE24 = 34008;
        public const int TEXTURE25 = 34009;
        public const int TEXTURE26 = 34010;
        public const int TEXTURE27 = 34011;
        public const int TEXTURE28 = 34012;
        public const int TEXTURE29 = 34013;
        public const int TEXTURE3 = 33987;
        public const int TEXTURE30 = 34014;
        public const int TEXTURE31 = 34015;
        public const int TEXTURE4 = 33988;
        public const int TEXTURE5 = 33989;
        public const int TEXTURE6 = 33990;
        public const int TEXTURE7 = 33991;
        public const int TEXTURE8 = 33992;
        public const int TEXTURE9 = 33993;
        public const int TRIANGLE_FAN = 6;
        public const int TRIANGLE_STRIP = 5;
        public const int TRIANGLES = 4;
        public const int UNPACK_ALIGNMENT = 3317;
        public const int UNSIGNED_BYTE = 5121;
        public const int UNSIGNED_INT = 5125;
        public const int UNSIGNED_SHORT = 5123;
        public const int UNSIGNED_SHORT_4_4_4_4 = 32819;
        public const int UNSIGNED_SHORT_5_5_5_1 = 32820;
        public const int UNSIGNED_SHORT_5_6_5 = 33635;
        public const int VALIDATE_STATUS = 35715;
        public const int VENDOR = 7936;
        public const int VERSION = 7938;
        public const int VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 34975;
        public const int VERTEX_ATTRIB_ARRAY_ENABLED = 34338;
        public const int VERTEX_ATTRIB_ARRAY_NORMALIZED = 34922;
        public const int VERTEX_ATTRIB_ARRAY_POINTER = 34373;
        public const int VERTEX_ATTRIB_ARRAY_SIZE = 34339;
        public const int VERTEX_ATTRIB_ARRAY_STRIDE = 34340;
        public const int VERTEX_ATTRIB_ARRAY_TYPE = 34341;
        public const int VERTEX_SHADER = 35633;
        public const int VIEWPORT = 2978;
        public const int ZERO = 0;

        public GL()
        {
        }

        [ScriptField]
        public CanvasElement canvas
        {
            get
            {
                return null;
            }
        }
        [ScriptField]
        public WebGLContextAttributes contextAttributes { get { return null; } }

        [ScriptField]
        public int error { get { return 0; } }

        public void activeTexture(int texture) { return; }
        public void attachShader(WebGLProgram program, WebGLShader shader) { return; }
        public void bindAttribLocation(WebGLProgram program, int index, string name) { return; }
        public void bindBuffer(int target, WebGLBuffer buffer) { return; }
        public void bindFramebuffer(int target, WebGLFramebuffer framebuffer) { return; }
        public void bindRenderbuffer(int target, WebGLRenderbuffer renderbuffer) { return; }
        public void bindTexture(int target, WebGLTexture texture) { return; }
        public void blendColor(float red, float green, float blue, float alpha) { return; }
        public void blendEquation(int mode) { return; }
        public void blendEquationSeparate(int modeRGB, int modeAlpha) { return; }
        public void blendFunc(int sfactor, int dfactor) { return; }
        public void blendFuncSeparate(int srcRGB, int dstRGB, int srcAlpha, int dstAlpha) { return; }
        public void bufferData(int target, int size, int usage) { return; }
        public void bufferData(int target, Float32Array data, int usage) { return; }
        public void bufferData(int target, Uint16Array data, int usage) { return; }
        public void bufferData(int target, Uint32Array data, int usage) { return; }
        public void bufferData(int target, WebGLArray data, int usage) { return; }
        public void bufferSubData(int target, int offset, WebGLArray data) { return; }
        public void bufferSubData(int target, int offset, WebGLArrayBuffer data) { return; }
        public int checkFramebufferStatus(int target) { return 0; }
        public void clear(int mask) { return; }
        public void clearColor(float red, float green, float blue, float alpha) { return; }
        public void clearDepth(float depth) { return; }
        public void clearStencil(int s) { return; }
        public void colorMask(bool red, bool green, bool blue, bool alpha) { return; }
        public void compileShader(WebGLShader shader) { return; }
        public void copyTexImage2D(int target, int level, int internalformat, int x, int y, int width, int height, int border) { return; }
        public void copyTexSubImage2D(int target, int level, int xoffset, int yoffset, int x, int y, int width, int height) { return; }
        public WebGLBuffer createBuffer() { return null; }
        public WebGLFramebuffer createFramebuffer() { return null; }
        public WebGLProgram createProgram() { return null; }
        public WebGLRenderbuffer createRenderbuffer() { return null; }
        public WebGLShader createShader(int type) { return null; }
        public WebGLTexture createTexture() { return null; }
        public void cullFace(int mode) { return; }
        public void deleteBuffer(WebGLBuffer buffer) { return; }
        public void deleteFramebuffer(WebGLFramebuffer framebuffer) { return; }
        public void deleteProgram(WebGLProgram program) { return; }
        public void deleteRenderbuffer(WebGLRenderbuffer renderbuffer) { return; }
        public void deleteShader(WebGLShader shader) { return; }
        public void deleteTexture(WebGLTexture texture) { return; }
        public void depthFunc(int func) { return; }
        public void depthMask(bool flag) { return; }
        public void depthRange(float zNear, float zFar) { return; }
        public void detachShader(WebGLProgram program, WebGLShader shader) { return; }
        public void disable(int cap) { return; }
        public void disableVertexAttribArray(int index) { return; }
        public void drawArrays(int mode, int first, int count) { return; }
        public void drawElements(int mode, int count, int type, int offset) { return; }
        public void enable(int cap) { return; }
        public void enableVertexAttribArray(int index) { return; }
        public void finish() { return; }
        public void flush() { return; }
        public void framebufferRenderbuffer(int target, int attachment, int renderbuffertarget, WebGLRenderbuffer renderbuffer) { return; }
        public void framebufferTexture2D(int target, int attachment, int textarget, WebGLTexture texture, int level) { return; }
        public void frontFace(int mode) { return; }
        public void generateMipmap(int target) { return; }
        public WebGLActiveInfo getActiveAttrib(WebGLProgram program, int index) { return null; }
        public WebGLActiveInfo getActiveUniform(WebGLProgram program, int index) { return null; }
        public WebGLObjectArray getAttachedShaders(WebGLProgram program) { return null; }
        public int getAttribLocation(WebGLProgram program, string name) { return 0; }
        public object getBufferParameter(int target, int pname) { return null; }
        public object getFramebufferAttachmentParameter(int target, int attachment, int pname) { return null; }
        public object getParameter(int pname) { return null; }
        public int getExtension(string name) { return 0; }
        public String getProgramInfoLog(WebGLProgram program) { return null; }
        public object getProgramParameter(WebGLProgram program, int pname) { return null; }
        public object getRenderbufferParameter(int target, int pname) { return null; }
        public String getShaderInfoLog(WebGLShader shader) { return null; }
        public object getShaderParameter(WebGLShader shader, int pname) { return null; }
        public String getShaderSource(WebGLShader shader) { return null; }
        public String getString(int name) { return null; }
        public object getTexParameter(int target, int pname) { return null; }
        public object getUniform(WebGLProgram program, WebGLUniformLocation location) { return null; }
        public WebGLUniformLocation getUniformLocation(WebGLProgram program, string name) { return null; }
        public object getVertexAttrib(int index, int pname) { return null; }
        public int getVertexAttribOffset(int index, int pname) { return 0; }
        public void hint(int target, int mode) { return; }
        public bool isBuffer(WebGLBuffer buffer) { return false; }
        public bool isContextLost() { return false; }
        public bool isEnabled(int cap) { return false; ;}
        public bool isFramebuffer(WebGLFramebuffer framebuffer) { return false; ;}
        public bool isProgram(WebGLProgram program) { return false; ;}
        public bool isRenderbuffer(WebGLRenderbuffer renderbuffer) { return false; ;}
        public bool isShader(WebGLShader shader) { return false; ;}
        public bool isTexture(WebGLTexture texture) { return false; ;}
        public void lineWidth(float width) { return; }
        public void linkProgram(WebGLProgram program) { return; }
        public void pixelStorei(int pname, int param) { return; }
        public void polygonOffset(float factor, float units) { return; }
        public WebGLArray readPixels(int x, int y, int width, int height, int format, int type) { return null; }
        public void renderbufferStorage(int target, int internalformat, int width, int height) { return; }
        public bool resetContext() { return false; }
        public void sampleCoverage(float value, bool invert) { return; }
        public void scissor(int x, int y, int width, int height) { return; }
        public void shaderSource(WebGLShader shader, string source) { return; }
        public void stencilFunc(int func, int @ref, int mask) { return; }
        public void stencilFuncSeparate(int face, int func, int @ref, int mask) { return; }
        public void stencilMask(int mask) { return; }
        public void stencilMaskSeparate(int face, int mask) { return; }
        public void stencilOp(int fail, int zfail, int zpass) { return; }
        public void stencilOpSeparate(int face, int fail, int zfail, int zpass) { return; }
        public void texImage2D(int target, int level, CanvasElement canvas) { return; }
        public void texImage2D(int target, int level, ImageElement image) { return; }
        public void texImage2D(int target, int level, VideoElement image) { return; }
        public void texImage2D(int target, int level, int format, int fmt, int storage, ImageElement video) { return; }
        public void texImage2D(int target, int level, ImageData pixels) { return; }
        public void texImage2D(int target, int level, CanvasElement canvas, bool flipY) { return; }
        public void texImage2D(int target, int level, ImageElement image, bool flipY) { return; }
        public void texImage2D(int target, int level, VideoElement video, bool flipY) { return; }
        public void texImage2D(int target, int level, ImageData pixels, bool flipY) { return; }
        public void texImage2D(int target, int level, CanvasElement canvas, bool flipY, bool asPremultipliedAlpha) { return; }
        public void texImage2D(int target, int level, ImageElement image, bool flipY, bool asPremultipliedAlpha) { return; }
        public void texImage2D(int target, int level, VideoElement video, bool flipY, bool asPremultipliedAlpha) { return; }
        public void texImage2D(int target, int level, ImageData pixels, bool flipY, bool asPremultipliedAlpha) { return; }
        public void texImage2D(int target, int level, int internalformat, int width, int height, int border, int format, int type, WebGLArray pixels) { return; }
        public void texParameterf(int target, int pname, float param) { return; }
        public void texParameteri(int target, int pname, int param) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, CanvasElement canvas) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, ImageElement image) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, VideoElement video) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, ImageData pixels) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, CanvasElement canvas, bool flipY) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, ImageElement image, bool flipY) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, VideoElement video, bool flipY) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, ImageData pixels, bool flipY) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, CanvasElement canvas, bool flipY, bool asPremultipliedAlpha) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, ImageElement image, bool flipY, bool asPremultipliedAlpha) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, VideoElement video, bool flipY, bool asPremultipliedAlpha) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, ImageData pixels, bool flipY, bool asPremultipliedAlpha) { return; }
        public void texSubImage2D(int target, int level, int xoffset, int yoffset, int width, int height, int format, int type, WebGLArray pixels) { return; }
        public void uniform1f(WebGLUniformLocation location, float x) { return; }
        public void uniform1fv(WebGLUniformLocation location, float[] v) { return; }
        public void uniform1fv(WebGLUniformLocation location, WebGLFloatArray v) { return; }
        public void uniform1i(WebGLUniformLocation location, int x) { return; }
        public void uniform1iv(WebGLUniformLocation location, int[] v) { return; }
        public void uniform1iv(WebGLUniformLocation location, WebGLIntArray v) { return; }
        public void uniform2f(WebGLUniformLocation location, float x, float y) { return; }
        public void uniform2fv(WebGLUniformLocation location, float[] v) { return; }
        public void uniform2fv(WebGLUniformLocation location, WebGLFloatArray v) { return; }
        public void uniform2i(WebGLUniformLocation location, int x, int y) { return; }
        public void uniform2iv(WebGLUniformLocation location, int[] v) { return; }
        public void uniform2iv(WebGLUniformLocation location, WebGLIntArray v) { return; }
        public void uniform3f(WebGLUniformLocation location, float x, float y, float z) { return; }
        public void uniform3fv(WebGLUniformLocation location, float[] v) { return; }
        public void uniform3fv(WebGLUniformLocation location, WebGLFloatArray v) { return; }
        public void uniform3i(WebGLUniformLocation location, int x, int y, int z) { return; }
        public void uniform3iv(WebGLUniformLocation location, int[] v) { return; }
        public void uniform3iv(WebGLUniformLocation location, WebGLIntArray v) { return; }
        public void uniform4f(WebGLUniformLocation location, float x, float y, float z, float w) { return; }
        public void uniform4fv(WebGLUniformLocation location, float[] v) { return; }
        public void uniform4fv(WebGLUniformLocation location, WebGLFloatArray v) { return; }
        public void uniform4i(WebGLUniformLocation location, int x, int y, int z, int w) { return; }
        public void uniform4iv(WebGLUniformLocation location, int[] v) { return; }
        public void uniform4iv(WebGLUniformLocation location, WebGLIntArray v) { return; }
        public void uniformMatrix2fv(WebGLUniformLocation location, bool transpose, float[] value) { return; }
        public void uniformMatrix2fv(WebGLUniformLocation location, bool transpose, WebGLFloatArray value) { return; }
        public void uniformMatrix3fv(WebGLUniformLocation location, bool transpose, float[] value) { return; }
        public void uniformMatrix3fv(WebGLUniformLocation location, bool transpose, WebGLFloatArray value) { return; }
        public void uniformMatrix4fv(WebGLUniformLocation location, bool transpose, float[] value) { return; }
        public void uniformMatrix4fv(WebGLUniformLocation location, bool transpose, WebGLFloatArray value) { return; }
        public void useProgram(WebGLProgram program) { return; }
        public void validateProgram(WebGLProgram program) { return; }
        public void vertexAttrib1f(int indx, float x) { return; }
        public void vertexAttrib1fv(int indx, float[] values) { return; }
        public void vertexAttrib1fv(int indx, WebGLFloatArray values) { return; }
        public void vertexAttrib2f(int indx, float x, float y) { return; }
        public void vertexAttrib2fv(int indx, float[] values) { return; }
        public void vertexAttrib2fv(int indx, WebGLFloatArray values) { return; }
        public void vertexAttrib3f(int indx, float x, float y, float z) { return; }
        public void vertexAttrib3fv(int indx, float[] values) { return; }
        public void vertexAttrib3fv(int indx, WebGLFloatArray values) { return; }
        public void vertexAttrib4f(int indx, float x, float y, float z, float w) { return; }
        public void vertexAttrib4fv(int indx, float[] values) { return; }
        public void vertexAttrib4fv(int indx, WebGLFloatArray values) { return; }
        public void vertexAttribPointer(int indx, int size, int type, bool normalized, int stride, int offset) { return; }
        public void viewport(int x, int y, int width, int height) { return; }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLContextAttributes
    {
        public WebGLContextAttributes()
        { }
        [ScriptField]
        public bool alpha { get { return false; } set { } }
        [ScriptField]
        public bool antialias { get { return false; } set { } }
        [ScriptField]
        public bool depth { get { return false; } set { } }
        [ScriptField]
        public bool premultipliedAlpha { get { return false; } set { } }
        [ScriptField]
        public bool stencil { get { return false; } set { } }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLProgram : WebGLObject
    {
        public WebGLProgram() { }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLObject
    {
        public WebGLObject() { }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLShader : WebGLObject
    {
        public WebGLShader() { }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLBuffer : WebGLObject
    {
        public WebGLBuffer() { }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLFramebuffer : WebGLObject
    {
        public WebGLFramebuffer() { }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLRenderbuffer : WebGLObject
    {
        public WebGLRenderbuffer() { }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLTexture : WebGLObject
    {
        public WebGLTexture() { }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLArray
    {
        public WebGLArray() { }

        [ScriptField]
        public WebGLArrayBuffer buffer { get { return null; } }
        [ScriptField]
        public int byteLength { get { return 0; } }
        [ScriptField]
        public int byteOffset { get { return 0; } }
        [ScriptField]
        public int length { get { return 0; } }

        public WebGLArray slice(int start, int end)
        {
            return null;
        }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLArrayBuffer
    {
        public WebGLArrayBuffer() { }

        [ScriptField]
        public int byteLength { get { return 0; } set { } }
        [ScriptField]
        public int size { get { return 0; } set { } }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLActiveInfo
    {
        public WebGLActiveInfo() { }
        [ScriptField]
        public String name { get { return null; } }
        [ScriptField]
        public int size { get { return 0; } }
        [ScriptField]
        public int type { get { return 0; } }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLObjectArray
    {
        public WebGLObjectArray() { }

        [ScriptField]
        public int length { get { return 0; } }

        public WebGLObject get(int index) { return null; }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLUniformLocation
    {
        public WebGLUniformLocation() { }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed class VideoElement : MediaElement
    {
        public VideoElement() { }
        [ScriptField]
        public String height { get { return null; } set { } }
        [ScriptField]
        public String poster { get { return null; } set { } }
        [ScriptField]
        public int videoHeight { get { return 0; } }
        [ScriptField]
        public int videoWidth { get { return 0; } }
        [ScriptField]
        public String width { get { return null; } set { } }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLFloatArray : WebGLArray
    {
        public static int BYTES_PER_ELEMENT;

        public WebGLFloatArray() { }

        public float get(int index) { return 0; }

        public void set(float[] array) { }

        public void set(WebGLFloatArray array) { }

        public void set(float[] array, int offset) { }

        public void set(int index, float value) { }

        public void set(WebGLFloatArray array, int offset) { }
    }


    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class WebGLIntArray : WebGLArray
    {
        public static int BYTES_PER_ELEMENT;

        public WebGLIntArray() { }

        public int get(int index) { return 0; }
        public void set(int[] array) { }
        public void set(WebGLIntArray array) { }
        public void set(int index, int value) { }
        public void set(int[] array, int offset) { }
        public void set(WebGLIntArray array, int offset) { }
    }


    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class MediaElement : Element
    {
        public static short HAVE_CURRENT_DATA;
        public static short HAVE_ENOUGH_DATA;
        public static short HAVE_FUTURE_DATA;
        public static short HAVE_METADATA;
        public static short HAVE_NOTHING;
        public static short NETWORK_EMPTY;
        public static short NETWORK_IDLE;
        public static short NETWORK_LOADING;
        public static short NETWORK_NO_SOURCE;

        public MediaElement() { }
        [ScriptField]
        public bool autoplay { get { return false; } set { } }
        [ScriptField]
        public TimeRanges buffered { get { return null; } }
        [ScriptField]
        public bool controls { get { return false; } set { } }
        [ScriptField]
        public String currentSrc { get { return null; } }
        [ScriptField]
        public float currentTime { get { return 0; } set { } }
        [ScriptField]
        public float defaultPlaybackRate { get { return 0; } set { } }
        [ScriptField]
        public float duration { get { return 0; } }
        [ScriptField]
        public bool ended { get { return false; } }
        [ScriptField]
        public MediaError error { get { return null; } }
        [ScriptField]
        public bool loop { get { return false; } set { } }
        [ScriptField]
        public bool muted { get { return false; } set { } }
        [ScriptField]
        public short networkStateget { get { return 0; } }
        [ScriptField]
        public bool paused { get { return false; } }
        [ScriptField]
        public float playbackRate { get { return 0; } set { } }
        [ScriptField]
        public TimeRanges playedget { get { return null; } }
        [ScriptField]
        public String preload { get { return null; } set { } }
        [ScriptField]
        public short readyState { get { return 0; } }
        [ScriptField]
        public TimeRanges seekable { get { return null; } }
        [ScriptField]
        public bool seeking { get { return false; } }
        [ScriptField]
        public String src { get { return null; } set { } }
        [ScriptField]
        public float startTime { get { return 0; } set { } }
        [ScriptField]
        public float volume { get { return 0; } set { } }


        public String canPlayType(string type) { return null; }
        public void load() { }
        public void pause() { }
        public void play() { }
    }
    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class TimeRanges
    {
        public TimeRanges() { }
        [ScriptField]
        public int length { get { return 0; } }

        public float end(int index) { return 0; }
        public float start(int index) { return 0; }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class MediaError
    {
        [ScriptField]
        public static short MEDIA_ERR_ABORTED { get { return 0; } }
        [ScriptField]
        public static short MEDIA_ERR_DECODE { get { return 0; } }
        [ScriptField]
        public static short MEDIA_ERR_NETWORK { get { return 0; } }
        [ScriptField]
        public static short MEDIA_ERR_SRC_NOT_SUPPORTED { get { return 0; } }

        public MediaError() { }

        [ScriptField]
        public short code { get { return 0; } }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class CrossDomainImage
    {
        [ScriptField]
        public string crossOrigin { get { return "anonymous"; } set { } }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class Float32Array
    {
        public Float32Array(object data) { }
        public Float32Array(object data, int a, int b) { }
        [ScriptField]
        public int length { get { return 0; } }

        [ScriptField]
        public float this[int key]
        {
            get { return 0; }
        }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class Float64Array
    {
        public Float64Array(object data) { }
        public Float64Array(object data, int a, int b) { }
        [ScriptField]
        public int length { get { return 0; } }

        [ScriptField]
        public double this[int key]
        {
            get { return 0; }
        }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class Uint8Array
    {
        public Uint8Array(object data) { }
        public Uint8Array(int size) { }
        [ScriptField]
        public int length { get { return 0; } }

        [ScriptField]
        public byte this[int key]
        {
            get { return 0; }
            set { }
        }

        [ScriptField]
        public object buffer { get { return null; } }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class Uint16Array
    {
        public Uint16Array(object data) { }
        [ScriptField]
        public int length { get { return 0; } }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public class Uint32Array
    {
        public Uint32Array(object data) { }
        [ScriptField]
        public int length { get { return 0; } }
    }
}



