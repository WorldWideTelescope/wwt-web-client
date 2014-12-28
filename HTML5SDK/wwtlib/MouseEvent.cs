using System;
using System.Runtime.CompilerServices;

namespace System.Html
{
    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed class MouseEvent
    {
        internal MouseEvent() {}

		[ScriptField]
        public int OffsetX { get { return 0; } set { } }

		[ScriptField]
        public int OffsetY { get { return 0; } set { } }

		[ScriptField]
        public int PageX { get { return 0; } set { } }

		[ScriptField]
        public int PageY { get { return 0; } set { } }

		[ScriptField]
        public int stylePaddingLeft { get { return 0; } set { } }

		[ScriptField]
        public int stylePaddingTop { get { return 0; } set { } }

		[ScriptField]
        public int styleBorderLeft { get { return 0; } set { } }

		[ScriptField]
        public int styleBorderTop { get { return 0; } set { } }

    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed class MouseCanvasElement
    {
        internal MouseCanvasElement() { }

		[ScriptField]
        public MouseCanvasElement offsetParent { get { return (MouseCanvasElement)this; } set { } }

		[ScriptField]
        public int offsetLeft { get { return 0; } set { } }

		[ScriptField]
        public int offsetTop { get { return 0; } set { } }
		

    }
}
