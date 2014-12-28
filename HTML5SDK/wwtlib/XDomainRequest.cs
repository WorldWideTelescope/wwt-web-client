using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace wwtlib
{
    [ScriptIgnoreNamespace]
    [ScriptImport]
    [ScriptName("XDomainRequest")]
    public sealed class XDomainRequest
    {
        public XDomainRequest() { }

        [ScriptField]
        public Action OnError { set { } }

        [ScriptField]
        public Action OnTimeout { set { } }

        [ScriptField]
        public Action OnProgress { set { } }

        [ScriptField]
        [ScriptName("onload")]
        public Action OnLoad { set{ } }

        [ScriptField]
        public int Timeout { get { return 0; } set { } }

        [ScriptField]
        [ScriptName("responseText")]
        public string ResponseText { get { return ""; } set { } }

        public void Open(string method, string url) { }
        public void Send() { }

    }
}
