// This contains a helper class 'CDN' which is used to create CDN URLs
// and allow the SDK caller to customize them.

using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    public class URLHelpers
    {
        public static string DEFAULT_HTTP_CDN = "cdn.worldwidetelescope.org";
        public static string DEFAULT_HTTPS_CDN = "beta.worldwidetelescope.org";

        public static string FromCDN(string path)
        {

            string protocol;
            string domain;

            protocol = (string)Script.Literal("window.location.protocol");

            switch (protocol) {
                case "http:":
                    domain = DEFAULT_HTTP_CDN;
                    break;
                case "https:":
                    domain = DEFAULT_HTTPS_CDN;
                    break;
                default:
                    domain = DEFAULT_HTTP_CDN;
                    break;
            }

            return "//" + domain + "/" + path;

        }

    }
}
