<%@ Page Language="C#" %>

<%
    
    string wwtlib = "wwtsdk.min.js";

    string[] values = Request.QueryString.GetValues("debug");
    if (values != null && values.Length > 0 && values[0] == "true")
    {
        wwtlib = "wwtsdk.js";
    }

    Response.WriteFile(wwtlib);

    //string mscorlib = "old/mscorlib.js";
    //string wwtlib = "old/wwtlib.js";

    //string[] values = Request.QueryString.GetValues("debug");
    //if (values != null && values.Length > 0 && values[0] == "true")
    //{
    //    mscorlib = "old/mscorlib.debug.js";
    //    wwtlib = "old/wwtlib.debug.js";
    //}

    //Response.WriteFile(mscorlib);
    //Response.Write(";");
    //Response.WriteFile(wwtlib);


%>

    