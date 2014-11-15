using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Configuration;

public partial class _Login : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
	   // if (Request.Cookies["alphakey"] != null && Request.Cookies["email"] != null)
	   // {
	   //     if (GoodieTwoShoes(Request.Cookies["email"].Value.ToLower()) == Request.Cookies["alphakey"].Value)
		//    {
			Response.Redirect("http://www.worldwidetelescope.org/webclient/default.aspx");
	   //     }

	  //  }


	}
	public static string GoodieTwoShoes(string factora)
	{
		MD5 md5 = new MD5CryptoServiceProvider();


		string sResult = "";
		string sSecret = "jsdkfjks3423345879(*&*&&fdsdf903e38278sd";
		string sStringIn = factora + sSecret;

		char[] caData = sStringIn.ToCharArray();
		byte[] data = new byte[caData.GetLength(0)];
		int i = 0;
		foreach (char c in caData)
		{
			data[i++] = (byte)c;
		}

		byte[] result = md5.ComputeHash(data);


		string sTemp;
		foreach (byte b in result)
		{
			sTemp = b.ToString("x");

			if (sTemp.Length == 1)
			{
				sTemp = "0" + sTemp;
			}
			sResult = sResult + sTemp;
		}

		char[] szNum = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };


		sResult = string.Format("{0}-{1}-{2}-{3}", sResult.Substring(3, 5), sResult.Substring(8, 5), sResult.Substring(16, 5), sResult.Substring(22, 5));
		return sResult.ToUpper();

	}

	protected void GetKey_Click(object sender, EventArgs e)
	{
		//if (!string.IsNullOrEmpty(this.emailadress.Text))
		//{
		//	string key = GoodieTwoShoes(this.emailadress.Text.Trim().ToLower());
		//	string body = string.Format("Please visit http://www.worldwidetelescope.org/webclient<br>E-Mail:{0}<br>Key:{1}", this.emailadress.Text, key);
		//	SendMail
		//	(
		//		ConfigurationManager.AppSettings["SmtpServer"],
		//		"wwtpage@microsoft.com",
		//		this.emailadress.Text.Trim().ToLower(),
		//		"WWT Key - Confidential",
		//		body
		//	);
		//	if (IsSafeDomain(this.emailadress.Text.Trim().ToLower()))
		//	{
		//		ErrorMessage.Text = "Your Key was send to your e-mail address";
		//	}
		//	else
		//	{
		//		ErrorMessage.Text = "Your e-mail address was send to an administrator for approval";
		//	}
		//}
		//else
		//{
		//	ErrorMessage.Text = "Please enter the e-mail adress";

		//}

	}
	protected void Login_Click(object sender, EventArgs e)
	{
		//if (GoodieTwoShoes(this.emailadress.Text.Trim().ToLower()) == key.Text.Trim())
		//{
		//	//Create a new cookie, passing the name into the constructor
		//	HttpCookie cookie = new HttpCookie("alphakey");

		//	//Set the cookies value
		//	cookie.Value = key.Text;

		//	DateTime dtNow = DateTime.Now;
		//	TimeSpan tsMinute = new TimeSpan(100, 0, 1, 0);
		//	cookie.Expires = dtNow + tsMinute;

		//	HttpCookie cookie2 = new HttpCookie("email");

		//	//Set the cookies value
		//	cookie2.Value = this.emailadress.Text;


		//	cookie2.Expires = dtNow + tsMinute;

		//	Response.Cookies.Add(cookie);
		//	Response.Cookies.Add(cookie2);


		//	//  Response.Redirect("");
		//	ErrorMessage.Text = "Your Key was written to the cookie Click the Banner above to start the Web Client";
		//Response.Redirect("http://www.worldwidetelescope.org/webclient/default.aspx");

		//}
		//else
		//{
		//	ErrorMessage.Text = "Invalid login information";
		//}
	}
	/// <summary>
	/// Generic SMPT Email Sender.
	/// </summary>
	/// <param name="SmtpServer"></param>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <param name="cc"></param>
	/// <param name="subject"></param>
	/// <param name="body"></param>
	protected void SendMail(string SmtpServer, string from, string to, string subject, string body)
	{
		MailAddress oFrom = new MailAddress(from);
		MailAddress oTo = new MailAddress(to);

		MailMessage message = new MailMessage();

		if (IsSafeDomain(to))
		{
			message.To.Add(oTo);
			message.Bcc.Add(oFrom);
		}
		else
		{
			message.To.Add(oFrom);
		}
		message.From = oFrom;
		message.Subject = subject;
		message.IsBodyHtml = true;


		message.Body = body;

		SmtpClient mailClient = new SmtpClient(SmtpServer);
		mailClient.Send(message);
	}
	protected bool IsSafeDomain(string email)
	{
		string domain = email.ToLower();

		if (domain.EndsWith("stsci.edu"))
		{
			return true;
		}
		if (domain.EndsWith("microsoft.com"))
		{
			return true;
		}
		if (domain.EndsWith("harvard.edu"))
		{
			return true;
		}
		if (domain.EndsWith("u.washington.edu"))
		{
			return true;
		}
		if (domain.EndsWith("nasa.gov"))
		{
			return true;
		}
		return false;

	}

}
