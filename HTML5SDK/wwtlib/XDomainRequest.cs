using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace wwtlib
{
    [IgnoreNamespace]
    [Imported]
    [ScriptName("XDomainRequest")]
    public sealed class XDomainRequest
    {
        public XDomainRequest() { }

        [IntrinsicProperty]
        public Action OnError { set { } }

        [IntrinsicProperty]
        public Action OnTimeout { set { } }

        [IntrinsicProperty]
        public Action OnProgress { set { } }

        [IntrinsicProperty]
        [ScriptName("onload")]
        public Action OnLoad { set{ } }

        [IntrinsicProperty]
        public int Timeout { get { return 0; } set { } }

        [IntrinsicProperty]
        [ScriptName("responseText")]
        public string ResponseText { get { return ""; } set { } }

        public void Open(string method, string url) { }
        public void Send() { }

    }
}
