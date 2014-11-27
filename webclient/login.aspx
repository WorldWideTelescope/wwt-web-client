<%@ Page Language="C#" EnableViewStateMac="false"  EnableViewState="false" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="_Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>WorldWide Telescope Alpha Registration</title>
<link href="../styles/wwt.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    
   
    <div>

                                    <table cellpadding="0" cellspacing="0" border="0" width="894">
                                    <tr><td>

 <div>
        <a href="http://www.worldwidetelescope.org/webclient/default.aspx" ><Img ID="Image1" 
            Src="http://www.worldwidetelescope.org/images/top_logo.jpg" /> </a>
        In order to access this pre-release you must agree to keep the site contents 
        confidential and provide your e-mail address.<br />
        If you don&#39;t yet have a key and agree to the terms, enter your e-mail adress and 
        click &quot;Get Key&quot; and a key will be send to your e-mail adress allowing you to 
        grant access to this browser.<br />
        <br />
        <br />
        <asp:Label ID="ErrorMessage" runat="server" 
            Text="If you already have a key enter it below and click login"></asp:Label>
    </div>  
</td>    
    <table>
    <tr> <td> <div> E-Mail address:</div></td>
               
        <td class="style1"> &nbsp;</td>
               
     <td><asp:TextBox ID="emailadress" runat="server" Width="176px"></asp:TextBox>
          </td>
               
     <td class="style2"><asp:Button ID="GetKey" runat="server" Text="Get Key" 
             onclick="GetKey_Click" />
          </td></tr>
    <tr><td>Key:&nbsp;</td><td class="style1">&nbsp;</td><td><asp:TextBox ID="key" runat="server" Width="176px"></asp:TextBox> </td>
        <td class="style2">&nbsp;</td></tr>
    <tr><td>&nbsp;</td><td class="style1">&nbsp;</td><td>
        <asp:Button ID="Login" runat="server" 
            Text="Login" Width="81px" onclick="Login_Click" /></td><td class="style2">&nbsp;</td></tr>

    </table>

    </div>
    </form>
</body>
</html>
