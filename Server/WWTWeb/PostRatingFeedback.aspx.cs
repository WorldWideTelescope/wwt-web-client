using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using WebServices;

public partial class PostRatingFeedback : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

  
    internal static SqlConnection GetConnectionWWTTours()
    {
        string connStr = null;
        connStr = ConfigurationManager.AppSettings["WWTToursDBConnectionString"];
        SqlConnection myConnection = null;
        myConnection = new SqlConnection(connStr);
        return myConnection;
    }


    
   

    public static void PostFeedback( string tour, string User, int rating)
    {

        string strErrorMsg;
        SqlConnection myConnection5 = GetConnectionWWTTours();

        try
        {
            myConnection5.Open();

            SqlCommand Cmd = null;
            Cmd = new SqlCommand();
            Cmd.CommandType = CommandType.StoredProcedure;
            Cmd.CommandTimeout = 20;
            Cmd.Connection = myConnection5;

            Cmd.CommandText = "spInsertTourRatingOrComment";

            SqlParameter CustParm = new SqlParameter("@pUserGUID", SqlDbType.VarChar);
            CustParm.Value = User.ToString();
            Cmd.Parameters.Add(CustParm);

            SqlParameter CustParm2 = new SqlParameter("@pTourGUID", SqlDbType.VarChar);
            CustParm2.Value = tour.ToString();
            Cmd.Parameters.Add(CustParm2);

            SqlParameter CustParm3 = new SqlParameter("@pRating", SqlDbType.VarChar);
            CustParm3.Value = rating;
            Cmd.Parameters.Add(CustParm3);

            Cmd.ExecuteNonQuery();


        }
        catch (InvalidCastException)
        { }

        catch (Exception ex)
        {
            //throw ex.GetBaseException();
            strErrorMsg = ex.Message;
            return ;
        }
        finally
        {
            if (myConnection5.State == ConnectionState.Open)
            {
                myConnection5.Close();
            }
        }

    }
}
