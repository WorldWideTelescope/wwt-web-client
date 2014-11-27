using System;
using System.Runtime.CompilerServices;

namespace System.Html
{
    [IgnoreNamespace]
    [Imported]
    public sealed class MouseEvent
    {
        internal MouseEvent() {}

        [IntrinsicProperty]
        public int OffsetX { get { return 0; } set { } }

        [IntrinsicProperty]
        public int OffsetY { get { return 0; } set { } }

        [IntrinsicProperty]
        public int PageX { get { return 0; } set { } }

        [IntrinsicProperty]
        public int PageY { get { return 0; } set { } }

        [IntrinsicProperty]
        public int stylePaddingLeft { get { return 0; } set { } }

        [IntrinsicProperty]
        public int stylePaddingTop { get { return 0; } set { } }

        [IntrinsicProperty]
        public int styleBorderLeft { get { return 0; } set { } }

        [IntrinsicProperty]
        public int styleBorderTop { get { return 0; } set { } }

    }

    [IgnoreNamespace]
    [Imported]
    public sealed class MouseCanvasElement
    {
        internal MouseCanvasElement() { }

        [IntrinsicProperty]
        public MouseCanvasElement offsetParent { get { return (MouseCanvasElement)this; } set { } }

        [IntrinsicProperty]
        public int offsetLeft { get { return 0; } set { } }

        [IntrinsicProperty]
        public int offsetTop { get { return 0; } set { } }

    }
}
