using System;
using System.Runtime.CompilerServices;

namespace System.Html
{
    [IgnoreNamespace]
    [Imported]

    public sealed class WheelEvent 
    {
        internal WheelEvent()
        {
        }
        [IntrinsicProperty]
        public double WheelDelta
        {
            get
            {
                return 0;
            }
        }
        [IntrinsicProperty]
        public double detail
        {
            get
            {
                return 0;
            }
        }
    }

    [IgnoreNamespace]
    [Imported]
    public sealed class ChromeNode
    {
        internal ChromeNode()
        {

        }
        [IntrinsicProperty]
        public string TextContent
        {
            get
            {
                return "";
            }
        }
    }

    [IgnoreNamespace]
    [Imported]
    public sealed class PointerEvent 
    {
        internal PointerEvent()
        {

        }

        [IntrinsicProperty]
        public int PointerId
        {
            get
            {
                return 0;
            }
        }

    }
}