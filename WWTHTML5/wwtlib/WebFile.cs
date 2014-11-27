using System;
using System.Collections.Generic;
using System.Xml;
using System.Net;

namespace wwtlib
{
    public enum StateType
    {
        Pending = 0,
        Received = 1,
        Error = 2
    }

    public class WebFile
    {
        private string _url;
        private string _message;
        private StateType _state;
        private string _data;
        private XDomainRequest xdr;
        private XmlHttpRequest xhr;

        public WebFile(string url)
        {
            _url = url;
        }

        public void Send()
        {
            //send the appropriate request based on browser type
            // have to do this because IE8 and 9 don't support CORS
            string version = Navigator.AppVersion;
            if (version.IndexOf("MSIE 8") > -1 || version.IndexOf("MSIE 9") > -1)
            {
                IECrossDomain();
            }
            else
            {
                CORS();
            }

            State = StateType.Pending;
        }

        public Action OnStateChange;
        public string Message { get { return _message; } }
        public StateType State 
        { 
            get { return _state; } 
            private set 
            {            
                _state = value;
                if (OnStateChange != null)
                {
                    OnStateChange();
                } 
            } 
        }

        private void LoadData(string textReceived)
        {
            // received data, set the state vars and send statechange
            _data = textReceived;
            State = StateType.Received;
        }

        private void Error()
        {
            _message = String.Format("Error encountered loading {0}", _url);
            State = StateType.Error;
        }

        private void TimeOut()
        {
            _message = String.Format("Timeout encountered loading {0}", _url);
            State = StateType.Error;
        }

        private void IECrossDomain()
        {
            xdr = new XDomainRequest();
            xdr.OnLoad = delegate()
            {
                LoadData(xdr.ResponseText);
            };
            xdr.OnTimeout = Error;
            xdr.OnError = TimeOut;
            xdr.Open("get", _url);
            xdr.Send();
        }

        private void CORS()
        {
            xhr = new XmlHttpRequest();
            try
            {
                xhr.Open(HttpVerb.Get, _url);
                xhr.OnReadyStateChange = delegate()
                {
                    if (xhr.ReadyState == ReadyState.Loaded)
                    {
                        LoadData(xhr.ResponseText);
                    }
                };
                xhr.Send();
            }
            catch (Exception err)
            {
                _message = err.Message;
                State = StateType.Error;
                throw (err);
            }
        }

        public string GetText()
        {
            return _data;
        }

        public XmlDocument GetXml()
        {
            return XmlDocumentParser.Parse(_data);
        }
    }
}
