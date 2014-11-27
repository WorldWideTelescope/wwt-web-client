using System;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Net;
using System.Html;


namespace wwtlib
{
    [IgnoreNamespace]
    [Imported]
    [ScriptName("XMLHttpRequest")] 
    public sealed class XMLHttpRequest2 : Element

    {
        public XMLHttpRequest2(){}

        [ScriptName("onreadystatechange")]
        [IntrinsicProperty]
        public Action OnReadyStateChange { get { return null; } set { } }
        [IntrinsicProperty]
        public ReadyState ReadyState { get { return ReadyState.Loaded; } }
        [IntrinsicProperty]
        public string ResponseText { get { return null; } }
        [ScriptName("responseXML")]
        [IntrinsicProperty]
        public XmlDocument ResponseXml { get { return null; } }
        [IntrinsicProperty]
        public int Status { get { return 0; } }
        [IntrinsicProperty]
        public string StatusText { get { return null; } }
        [IntrinsicProperty] 
        public object Response { get { return null; } }
        [IntrinsicProperty]
        public string ResponseType { get { return null; } set { } }
        public void Abort() { }
        public string GetAllResponseHeaders() { return null; }
        public string GetResponseHeader(string name) { return null;}
        public void Open(HttpVerb verb, string url) { }
        public void Open(string method, string url) { }
        public void Open(HttpVerb verb, string url, bool async) { }
        public void Open(string method, string url, bool async) { }
        public void Open(HttpVerb verb, string url, bool async, string userName, string password) { }
        public void Open(string method, string url, bool async, string userName, string password) { }
        public void Send() { }
        public void Send(string body) { }
        public void SetRequestHeader(string name, string value) { }
    }
}