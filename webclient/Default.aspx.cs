using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using WURFL;
using Wurfl.Aspnet.Extensions.Request;

public partial class Default : System.Web.UI.Page
{
	public bool Debug = false;
	public bool DebugChrome = false;
	public bool ADS = false;
	public string ResourcesLocation = ConfigurationManager.AppSettings["ResourcesLocation"];
	public string ResourcesVersion = ConfigurationManager.AppSettings["ResourcesVersion"];
	public string DebugQs = "?v=" + ConfigurationManager.AppSettings["ResourcesVersion"];
	public string BodyClass;
	public string SDKLocation = "http://www.worldwidetelescope.org/scripts/wwtsdk.aspx";
	public enum Clients
	{
		Html5 = 0,
		Silverlight = 1,
		WWT = 2,
		Mobile = 3
	};
	public Clients Client = Clients.Html5;

	protected void Page_Load(object sender, EventArgs e)
	{
		var device = WURFLManagerBuilder.Instance.GetDeviceForRequest(Request.UserAgent);
		
		bool isMobile = device.GetCapability("is_smartphone") == "true";

		BodyClass = string.Format("fs-player wwt-webclient-wrapper {0}", isMobile ? "mobile" : "desktop");

		if (Request.QueryString["debug"] != null)
		{
			DebugQs = "?debug=true&v=" + ResourcesVersion;
			Debug = true;
			if (Request.QueryString["debug"] == "chrome")
			{
				Debug = false;
				DebugChrome = true;
				DebugQs = "";
			}
			if (Request.QueryString["debug"] == "local")
			{
				SDKLocation = "/sdk/wwtsdk.aspx";
			}
		}
		if (Request.QueryString["ads"] != null)
		{
			ADS = true;
		}
		if (Request.Cookies["preferredClient"] != null)
		{
			switch (Request.Cookies["preferredClient"].Value)
			{
				case "SL":
					Client = Clients.Silverlight;
					break;
				case "WWT":
					Client = Clients.WWT;
					break;
				case "Mobile":
					Client = Clients.Mobile;
					break;
				default:
					Client = Clients.Html5;
					break;
			}
		}
		if (Request.QueryString["client"] != null)
		{
			HttpCookie cookie = Request.Cookies["preferredClient"] ?? new HttpCookie("preferredClient");
			char c = Request.QueryString["client"].ToString(CultureInfo.InvariantCulture).ToLower().ToCharArray()[0];
			if (c == 'h')
			{
				Client = Clients.Html5;
				cookie.Value = "HTML5";
			}
			else if (c == 's')
			{
				Client = Clients.Silverlight;
				cookie.Value = "SL";
			}
			else if (c == 'm')
			{
				Client = Clients.Mobile;
				cookie.Value = "Mobile";
			}
			else if (c == 'w')
			{
				Client = Clients.WWT;
				cookie.Value = "WWT";
			}


			HttpContext.Current.Response.Cookies.Add(cookie);

		}

		if (Client == Clients.Html5 && isMobile)
		{
			Response.Redirect(string.Format("/webclient/?client=mobile{0}", Debug ? "&debug=true" : ""));
		}
		else if (Client == Clients.Mobile && !isMobile)
		{
			Response.Redirect(string.Format("/webclient/?client=html5{0}", Debug ? "&debug=true" : ""));
		}
	}
}