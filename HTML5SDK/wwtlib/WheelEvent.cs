using System;
using System.Runtime.CompilerServices;

namespace System.Html
{
    [ScriptIgnoreNamespace]
    [ScriptImport]

    public sealed class WheelEvent 
    {
        internal WheelEvent()
        {
        }
        [ScriptField]
        public double WheelDelta
        {
            get
            {
                return 0;
            }
        }
        [ScriptField]
        public double detail
        {
            get
            {
                return 0;
            }
        }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed class ChromeNode
    {
        internal ChromeNode()
        {

        }
        [ScriptField]
        public string TextContent
        {
            get
            {
                return "";
            }
        }
    }

    [ScriptIgnoreNamespace]
    [ScriptImport]
    public sealed class PointerEvent 
    {
        internal PointerEvent()
        {

        }

        [ScriptField]
        public int PointerId
        {
            get
            {
                return 0;
            }
        }

    }
}