// This contains a helper class 'CDN' which is used to create CDN URLs
// and allow the SDK caller to customize them.

using System;
using System.Collections.Generic;
using System.Linq;


namespace wwtlib
{
    public class URLHelpers
    {

        // NOTE: we deliberately don't define the default URLs as static
        // properties here since they end up not being set in time for some
        // calls to FromCDN and FromWWT

        public static string FromCDN(string path)
        {

            string protocol;
            string domain;

            protocol = (string)Script.Literal("window.location.protocol");

            switch (protocol) {
                case "http:":
                    domain = "cdn.worldwidetelescope.org";
                    break;
                case "https:":
                    domain = "beta.worldwidetelescope.org";
                    break;
                default:
                    domain = "cdn.worldwidetelescope.org";
                    break;
            }

            return "//" + domain + "/" + path;

        }

        public static string FromWWW(string path)
        {

            string protocol;
            string domain;

            protocol = (string)Script.Literal("window.location.protocol");

            switch (protocol) {
                case "http:":
                    domain = "www.worldwidetelescope.org";
                    break;
                case "https:":
                    domain = "beta.worldwidetelescope.org";
                    break;
                default:
                    domain = "www.worldwidetelescope.org";
                    break;
            }

            return "//" + domain + "/" + path;

        }

        public static string Patch(string url)
        {

            string protocol;
            protocol = (string)Script.Literal("window.location.protocol");

            if (protocol == "http:") {
                return url;
            }

            if (url.StartsWith("http://worldwidetelescope.org")) {
                url = url.Replace("http://worldwidetelescope.org/", "");
                return FromWWW(url);
            }

            if (url.StartsWith("http://www.worldwidetelescope.org")) {
                url = url.Replace("http://www.worldwidetelescope.org/", "");
                return FromWWW(url);
            }

            if (url.StartsWith("http://cdn.worldwidetelescope.org")) {
                url = url.Replace("http://cdn.worldwidetelescope.org/", "");
                return FromCDN(url);
            }

            if (url.StartsWith("http://")) {
                url = url.Replace("http://", "https://");
            }

            return url;

        }

    }
}
