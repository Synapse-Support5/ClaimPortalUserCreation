using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ClaimPortalUserCreation
{
    public partial class Login : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["SqlConn"].ToString());
        DataTable resdt = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string userId = UserId.Text;
                //string password = Password.Text;
                string encryptedPassword = Password.Text.Trim();
                string decryptedPassword = DecryptAES(encryptedPassword);

                if (userId == "")
                {
                    showToast("User Id is mandatory", "toast-danger");
                    return;
                }
                //else if (password == "")
                //{
                //    showToast("Password is mandatory", "toast-danger");
                //    return;
                //}

                //if (con.State == ConnectionState.Closed)
                //{
                //    con.Open();
                //}
                //SqlCommand cmd1 = new SqlCommand("SP_BBoard_Dropdowns", con);
                //cmd1.CommandType = CommandType.StoredProcedure;
                //cmd1.Parameters.AddWithValue("@session_Name", userId);
                //cmd1.Parameters.AddWithValue("@Password", decryptedPassword);
                //cmd1.Parameters.AddWithValue("@ActionType", "Session");

                //cmd1.CommandTimeout = 6000;
                //SqlDataAdapter da = new SqlDataAdapter(cmd1);
                //da.Fill(resdt);

                //if (resdt.Rows.Count > 0)
                //{
                //    Session["UserId"] = resdt.Rows[0][0].ToString();
                //    Session["BusinessType"] = resdt.Rows[0][2].ToString();
                //    Session["Role"] = resdt.Rows[0][3].ToString();

                //    Response.Redirect("Index.aspx", false);
                //    Context.ApplicationInstance.CompleteRequest();

                //    //string userId2 = HttpUtility.UrlEncode(Session["UserId"].ToString());
                //    //string businessType = HttpUtility.UrlEncode(Session["BusinessType"].ToString());
                //    //string role = HttpUtility.UrlEncode(Session["Role"].ToString());

                //    //// Redirect with query parameters
                //    //Response.Redirect($"Index.aspx?UserId={userId2}&BusinessType={businessType}&Role={role}");
                //}
                //else
                //{
                //    showToast("User not found", "toast-danger");
                //}

                // ✅ Generate one-time token
                string token = Guid.NewGuid().ToString();
                string sessionId = Session.SessionID;

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("SP_BBoard_Dropdowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", userId);
                cmd1.Parameters.AddWithValue("@Password", decryptedPassword);
                cmd1.Parameters.AddWithValue("@ActionType", "Session");

                cmd1.CommandTimeout = 6000;
                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                da.Fill(resdt);

                if (resdt.Rows.Count > 0)
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand cmd2 = new SqlCommand("SP_UserSessionTokens", con);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@ActionType", "Generate");
                    cmd2.Parameters.AddWithValue("@UserId", userId);
                    cmd2.Parameters.AddWithValue("@PageURL", "ClaimPortalUserCreation");
                    cmd2.Parameters.AddWithValue("@Token", token);
                    cmd2.Parameters.AddWithValue("@IsUsed", 0);
                    cmd2.Parameters.AddWithValue("@SessionId", sessionId);

                    cmd2.CommandTimeout = 6000;
                    cmd2.ExecuteNonQuery();

                    Session["UserId"] = resdt.Rows[0][0].ToString();
                    Session["BusinessType"] = resdt.Rows[0][2].ToString();
                    Session["Role"] = resdt.Rows[0][3].ToString();

                    string redirectUrl = $"Index.aspx?token={token}";
                    Response.Redirect(redirectUrl, false);
                    Context.ApplicationInstance.CompleteRequest();

                }
                else
                {
                    showToast("User not found", "toast-danger");
                }
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }

        private string DecryptAES(string cipherText)
        {
            try
            {
                cipherText = System.Web.HttpUtility.UrlDecode(cipherText);
                cipherText = cipherText.Replace(" ", "+");

                string key = "MySuperSecretKeyForAES256_123456"; // 32 chars = 256-bit key
                string iv = "MyInitVector1234";                   // 16 chars = 128-bit IV

                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = Encoding.UTF8.GetBytes(iv);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (MemoryStream ms = new MemoryStream(cipherBytes))
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (StreamReader reader = new StreamReader(cs))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (FormatException fex)
            {
                System.Diagnostics.Debug.WriteLine("Base64 format error: " + fex.ToString());
                throw new Exception("Invalid encrypted password format.");
            }
            catch (CryptographicException cex)
            {
                System.Diagnostics.Debug.WriteLine("AES decrypt error: " + cex.ToString());
                throw new Exception("Decryption failed. Invalid key or data.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("General decrypt error: " + ex.ToString());
                throw new Exception("Unexpected error during decryption.");
            }
        }

        #region ToastNotification
        private void showToast(string message, string styleClass)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showToast", $"showToast('{message}', '{styleClass}');", true);
        }
        #endregion

    }

}