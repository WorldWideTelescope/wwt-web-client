<%@ Application Language="C#" %>
<%@ Import Namespace="WURFL" %>
<%@ Import Namespace="WURFL.Config" %>

<script runat="server">
	public const String WurflDataFilePath = "./App_Data/wurfl-latest.zip";
	void Application_Start(object sender, EventArgs e) 
	{
		var wurflDataFile = HttpContext.Current.Server.MapPath(WurflDataFilePath);

		var configurer = new InMemoryConfigurer()
			.MainFile(wurflDataFile);
		WURFLManagerBuilder.Build(configurer);

	}
	
	void Application_End(object sender, EventArgs e) 
	{
		//  Code that runs on application shutdown

	}
		
	void Application_Error(object sender, EventArgs e) 
	{ 
		// Code that runs when an unhandled error occurs

	}

	void Session_Start(object sender, EventArgs e) 
	{
		// Code that runs when a new session is started

	}

	void Session_End(object sender, EventArgs e) 
	{
		// Code that runs when a session ends. 
		// Note: The Session_End event is raised only when the sessionstate mode
		// is set to InProc in the Web.config file. If session mode is set to StateServer 
		// or SQLServer, the event is not raised.

	}
	   
</script>
