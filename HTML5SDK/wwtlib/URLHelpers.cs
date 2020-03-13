// This contains a helper class 'CDN' which is used to create CDN URLs
// and allow the SDK caller to customize them.

using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    public class URL
    {
        public float A = 255.0f;
        public float B = 255.0f;
        public float G = 255.0f;
        public float R = 255.0f;
        public string DEFAULT_HTTP_CDN = "cdn.worldwidetelescope.org";
        public string DEFAULT_HTTPS_CDN = "beta.worldwidetelescope.org";

        public static string FromCDN(string path)
        {

            string protocol;
            string domain;

            protocol = (string)Script.Literal("window.location.protocol");

            switch (protocol) {
                case "http":
                    domain = URL.DEFAULT_HTTP_CDN;
                    break;
                case "https":
                    domain = URL.DEFAULT_HTTPS_CDN;
                    break;
                default:
                    domain = URL.DEFAULT_HTTP_CDN;
                    break;
            }

            return "//" + domain + "/" + path;

        }

    }
}
