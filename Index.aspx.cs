using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using Microsoft.Ajax.Utilities;

namespace ClaimPortalUserCreation
{
    public partial class Index : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["SqlConn"].ToString());
        DataTable dt = new DataTable();
        DataTable resdt = new DataTable();
        DataSet ds1 = new DataSet();

        #region PageLoad
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (Session["UserId"] == null || Session["BusinessType"] == null || Session["Role"] == null)
                //{
                //    Response.Redirect("Login.aspx");
                //}
                //else
                //{
                //    string userId = Session["UserId"].ToString();
                //    string businessType = Session["BusinessType"].ToString();
                //    string role = Session["Role"].ToString();

                //    Session["name"] = userId;

                //    lblUserName.Text = "Welcome : " + userId;
                //    //hdnBusinessType.Value = businessType;
                //    //hdnRole.Value = role;

                //    //AccessLoad();
                //    StateLoad();
                //}

                string token = Request.QueryString["token"];
                string sessionId = Session.SessionID;

                // 1️⃣ Token missing — redirect
                if (string.IsNullOrEmpty(token))
                {
                    showToast("Invalid session. Please login again.", "toast-danger");
                    Response.Redirect("Login.aspx", true);
                    return;
                }

                // 2️⃣ Validate token in DB
                string userId = "";
                string businessType = "";
                string role = "";


                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd2 = new SqlCommand("SP_UserSessionTokens", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.AddWithValue("@ActionType", "Validate");
                cmd2.Parameters.AddWithValue("@UserId", userId);
                cmd2.Parameters.AddWithValue("@PageURL", "ClaimPortalUserCreation");
                cmd2.Parameters.AddWithValue("@Token", token);
                cmd2.Parameters.AddWithValue("@IsUsed", 0);
                cmd2.Parameters.AddWithValue("@SessionId", sessionId);

                SqlDataAdapter da = new SqlDataAdapter(cmd2);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    // Invalid or already used token
                    showToast("Session expired. Please login again.", "toast-danger");
                    Response.Redirect("Login.aspx", true);
                    return;
                }

                // Token is valid
                userId = dt.Rows[0]["UserId"].ToString();
                businessType = dt.Rows[0]["BusinessType"].ToString();
                role = dt.Rows[0]["Role"].ToString();

                // ✅ 3️⃣ Mark token as used (one-time)
                SqlCommand cmdUpdate = new SqlCommand("SP_UserSessionTokens", con);
                cmdUpdate.CommandType = CommandType.StoredProcedure;
                cmdUpdate.Parameters.AddWithValue("@ActionType", "Update");
                cmdUpdate.Parameters.AddWithValue("@UserId", userId);
                cmdUpdate.Parameters.AddWithValue("@PageURL", "ClaimPortalUserCreation");
                cmdUpdate.Parameters.AddWithValue("@Token", token);
                cmdUpdate.Parameters.AddWithValue("@IsUsed", 0);
                cmdUpdate.Parameters.AddWithValue("@SessionId", sessionId);
                cmdUpdate.ExecuteNonQuery();


                // ✅ 4️⃣ Set session values
                Session["UserId"] = userId;
                Session["BusinessType"] = businessType;
                Session["Role"] = role;
                Session["name"] = userId;

                lblUserName.Text = "Welcome, " + userId;
                //hdnBusinessType.Value = businessType;
                //hdnRole.Value = role;

                // ✅ 5️⃣ Load main data
                StateLoad();
            }
        }
        #endregion

        #region AccessLoad
        public void AccessLoad()
        {
            try
            {
                Session["name"] = "G116036";
                //Session["name"] = Request.ServerVariables["REMOTE_USER"].Substring(6);

                if (Session["name"].ToString() != "")
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                    cmd1.Parameters.AddWithValue("@ActionType", "AccessLoad");
                    cmd1.Parameters.AddWithValue("@StateID", "");
                    cmd1.Parameters.AddWithValue("@AreaID", "");
                    cmd1.Parameters.AddWithValue("@ZoneId", "");
                    cmd1.Parameters.AddWithValue("@UserType", "");
                    cmd1.Parameters.AddWithValue("@UserId", "");
                    cmd1.Parameters.AddWithValue("@UserName", "");
                    cmd1.Parameters.AddWithValue("@UserEmail", "");
                    cmd1.Parameters.AddWithValue("@UserMobile", "");
                    cmd1.Parameters.AddWithValue("@Password", "");

                    cmd1.CommandTimeout = 6000;
                    SqlDataAdapter da = new SqlDataAdapter(cmd1);
                    da.Fill(resdt);

                    if (resdt.Rows.Count > 0)
                    {
                        //lblUserName.Text = "User Name > " + resdt.Rows[0][0].ToString() + ": User ID > " + Session["name"].ToString();
                        lblUserName.Text = "Welcome : " + resdt.Rows[0][0].ToString();
                        lblUserName1.Text = "Welcome : " + resdt.Rows[0][0].ToString();
                    }
                    else
                    {
                        Response.Redirect("AccessDeniedPage.aspx");
                    }
                    Session["Role"] = resdt.Rows[0][3].ToString();
                    con.Close();
                }
                else
                {
                    Response.Redirect("AccessDeniedPage.aspx");
                }
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }
        #endregion

        #region SelectedIndexChanged
        protected void StateDrp_SelectedIndexChanged(object sender, EventArgs e)
        {
            AreaLoad();
        }

        protected void AreaDrp_SelectedIndexChanged(object sender, EventArgs e)
        {
            ZoneLoad();
        }

        protected void ZoneDrp_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserTypeLoad();
        }

        protected void UserId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CPGridView.DataSource = null;
                CPGridView.DataBind();

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                cmd1.Parameters.AddWithValue("@ActionType", "CheckData");
                cmd1.Parameters.AddWithValue("@StateID", "");
                cmd1.Parameters.AddWithValue("@AreaID", "");
                cmd1.Parameters.AddWithValue("@ZoneID", "");
                cmd1.Parameters.AddWithValue("@UserType", UserTypeDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@UserId", UserId.Text);
                cmd1.Parameters.AddWithValue("@UserName", "");
                cmd1.Parameters.AddWithValue("@UserEmail", "");
                cmd1.Parameters.AddWithValue("@UserMobile", "");
                cmd1.Parameters.AddWithValue("@Password", "");
                cmd1.CommandTimeout = 6000;
                cmd1.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    string responseMessage = dt.Rows[0]["Response"].ToString();
                    if (responseMessage == "1")
                    {
                        HtmlGenericControl confimDivDiv = (HtmlGenericControl)FindControl("ConfirmDiv");

                        confimDivDiv.Style["display"] = "block"; // Show confimDivDiv
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }

        }

        protected void UserTypeDrp_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                cmd1.Parameters.AddWithValue("@ActionType", "LoadGrid");
                cmd1.Parameters.AddWithValue("@StateID", StateDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@AreaID", AreaDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@ZoneID", ZoneDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@UserType", UserTypeDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@UserId", "");
                cmd1.Parameters.AddWithValue("@UserName", "");
                cmd1.Parameters.AddWithValue("@UserEmail", "");
                cmd1.Parameters.AddWithValue("@UserMobile", "");
                cmd1.Parameters.AddWithValue("@Password", "");
                cmd1.CommandTimeout = 6000;
                cmd1.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                ds1.Clear();
                da.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 1)
                {
                    CPGridView.DataSource = ds1.Tables[0];
                    CPGridView.DataBind();
                }
                else
                {
                    CPGridView.DataSource = null;
                    CPGridView.DataBind();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }

        protected void UserName_TextChanged(object sender, EventArgs e)
        {
            HtmlGenericControl confimDivDiv = (HtmlGenericControl)FindControl("ConfirmDiv");

            confimDivDiv.Style["display"] = "none"; // Hide confimDivDiv
        }
        #endregion

        #region StateLoad
        public void StateLoad()
        {
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                cmd1.Parameters.AddWithValue("@ActionType", "StateLoad");
                cmd1.Parameters.AddWithValue("@StateID", "");
                cmd1.Parameters.AddWithValue("@AreaID", "");
                cmd1.Parameters.AddWithValue("@ZoneId", "");
                cmd1.Parameters.AddWithValue("@UserType", "");
                cmd1.Parameters.AddWithValue("@UserId", "");
                cmd1.Parameters.AddWithValue("@UserName", "");
                cmd1.Parameters.AddWithValue("@UserEmail", "");
                cmd1.Parameters.AddWithValue("@UserMobile", "");
                cmd1.Parameters.AddWithValue("@Password", "");
                cmd1.ExecuteNonQuery();

                cmd1.CommandTimeout = 6000;

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                resdt.Rows.Clear();
                da.Fill(resdt);
                StateDrp.DataSource = resdt;
                StateDrp.DataTextField = resdt.Columns["StateName"].ToString();
                StateDrp.DataValueField = resdt.Columns["StateId"].ToString();
                StateDrp.DataBind();
                StateDrp.Items.Insert(0, new ListItem("State", ""));
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }

        }
        #endregion

        #region AreaLoad
        public void AreaLoad()
        {
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                cmd1.Parameters.AddWithValue("@ActionType", "AreaLoad");
                cmd1.Parameters.AddWithValue("@StateID", StateDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@AreaID", "");
                cmd1.Parameters.AddWithValue("@ZoneId", "");
                cmd1.Parameters.AddWithValue("@UserType", "");
                cmd1.Parameters.AddWithValue("@UserId", "");
                cmd1.Parameters.AddWithValue("@UserName", "");
                cmd1.Parameters.AddWithValue("@UserEmail", "");
                cmd1.Parameters.AddWithValue("@UserMobile", "");
                cmd1.Parameters.AddWithValue("@Password", "");
                cmd1.ExecuteNonQuery();

                cmd1.CommandTimeout = 6000;

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                resdt.Rows.Clear();
                da.Fill(resdt);
                AreaDrp.DataSource = resdt;
                AreaDrp.DataTextField = resdt.Columns["AreaName"].ToString();
                AreaDrp.DataValueField = resdt.Columns["AreaId"].ToString();
                AreaDrp.DataBind();
                AreaDrp.Items.Insert(0, new ListItem("Area", ""));
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }

        }
        #endregion

        #region ZoneLoad
        public void ZoneLoad()
        {
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                cmd1.Parameters.AddWithValue("@ActionType", "ZoneLoad");
                cmd1.Parameters.AddWithValue("@StateID", StateDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@AreaID", AreaDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@ZoneId", AreaDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@UserType", "");
                cmd1.Parameters.AddWithValue("@UserId", "");
                cmd1.Parameters.AddWithValue("@UserName", "");
                cmd1.Parameters.AddWithValue("@UserEmail", "");
                cmd1.Parameters.AddWithValue("@UserMobile", "");
                cmd1.Parameters.AddWithValue("@Password", "");
                cmd1.ExecuteNonQuery();

                cmd1.CommandTimeout = 6000;

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                resdt.Rows.Clear();
                da.Fill(resdt);
                ZoneDrp.DataSource = resdt;
                ZoneDrp.DataTextField = resdt.Columns["DistrictName"].ToString();
                ZoneDrp.DataValueField = resdt.Columns["DistrictId"].ToString();
                ZoneDrp.DataBind();
                ZoneDrp.Items.Insert(0, new ListItem("Zone", ""));
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }

        }
        #endregion

        #region UserTypeLoad
        public void UserTypeLoad()
        {
            try
            {

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                cmd1.Parameters.AddWithValue("@ActionType", "UserTypeLoad");
                cmd1.Parameters.AddWithValue("@StateID", "");
                cmd1.Parameters.AddWithValue("@AreaID", "");
                cmd1.Parameters.AddWithValue("@ZoneId", "");
                cmd1.Parameters.AddWithValue("@UserType", "");
                cmd1.Parameters.AddWithValue("@UserId", "");
                cmd1.Parameters.AddWithValue("@UserName", "");
                cmd1.Parameters.AddWithValue("@UserEmail", "");
                cmd1.Parameters.AddWithValue("@UserMobile", "");
                cmd1.Parameters.AddWithValue("@Password", "");
                cmd1.ExecuteNonQuery();

                cmd1.CommandTimeout = 6000;

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                resdt.Rows.Clear();
                da.Fill(resdt);
                UserTypeDrp.DataSource = resdt;
                UserTypeDrp.DataTextField = resdt.Columns["UserType"].ToString();
                UserTypeDrp.DataValueField = resdt.Columns["UserTypeId"].ToString();
                UserTypeDrp.DataBind();
                UserTypeDrp.Items.Insert(0, new ListItem("UserType", ""));
                con.Close();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }

        }
        #endregion

        #region BtnSubmit_Click
        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            BtnSubmit();
        }

        public void BtnSubmit()
        {
            try
            {
                string mob = UserMob.Text;

                //if (string.IsNullOrEmpty(mob))
                //{
                //    showToast("Mobile No. should not be empty", "toast-danger");
                //    return;
                //}
                //else if (!Regex.IsMatch(mob, @"^\d+$"))
                //{
                //    showToast("Mobile No. should be numeric", "toast-danger");
                //    return;
                //}
                //else if (mob.Length > 10)
                //{
                //    showToast("Mobile No. should not be more than 10 digits", "toast-danger");
                //    return;
                //}
                //else if (mob.Length < 10)
                //{
                //    showToast("Mobile No. should not be less than 10 digits", "toast-danger");
                //    return;
                //}

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                cmd1.Parameters.AddWithValue("@ActionType", "Create");
                cmd1.Parameters.AddWithValue("@StateID", StateDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@AreaID", AreaDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@ZoneID", ZoneDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@UserType", UserTypeDrp.SelectedValue);
                cmd1.Parameters.AddWithValue("@UserId", UserId.Text);
                cmd1.Parameters.AddWithValue("@UserName", UserName.Text);
                cmd1.Parameters.AddWithValue("@UserEmail", UserEmail.Text);
                cmd1.Parameters.AddWithValue("@UserMobile", mob);
                cmd1.Parameters.AddWithValue("@Password", UserPass.Text);
                cmd1.CommandTimeout = 6000;
                //cmd1.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                dt.Clear();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    string responseMessage = dt.Rows[0]["Response"].ToString();
                    if (responseMessage.StartsWith("Sorry1"))
                    {
                        showToast("Dist " + UserId.Text + " is not registered", "toast-danger");
                    }
                    else if (responseMessage.StartsWith("Sorry2"))
                    {
                        showToast("SO " + UserId.Text + " is not registered", "toast-danger");
                    }
                    else
                    {
                        showToast(responseMessage, "toast-success");
                    }
                }
                con.Close();

                ClearForm();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }

        }
        #endregion

        #region Confirm_Click
        protected void Confirm_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.ID == "ConfirmYes")
            {
                // Show EditDiv
                HtmlGenericControl editDiv = (HtmlGenericControl)FindControl("EditDiv");
                if (editDiv != null)
                    editDiv.Style["display"] = "block";

                // Hide ConfirmDiv and CreateDiv 
                HtmlGenericControl confirmDiv = (HtmlGenericControl)FindControl("ConfirmDiv");
                if (confirmDiv != null)
                    confirmDiv.Style["display"] = "none";
                HtmlGenericControl createDiv = (HtmlGenericControl)FindControl("CreateDiv");
                if (createDiv != null)
                    createDiv.Style["display"] = "none";

                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                    cmd1.Parameters.AddWithValue("@ActionType", "ModifyLoad");
                    cmd1.Parameters.AddWithValue("@StateID", "");
                    cmd1.Parameters.AddWithValue("@AreaID", "");
                    cmd1.Parameters.AddWithValue("@ZoneID", "");
                    cmd1.Parameters.AddWithValue("@UserType", UserTypeDrp.SelectedValue);
                    cmd1.Parameters.AddWithValue("@UserId", UserId.Text);
                    cmd1.Parameters.AddWithValue("@UserName", "");
                    cmd1.Parameters.AddWithValue("@UserEmail", "");
                    cmd1.Parameters.AddWithValue("@UserMobile", "");
                    cmd1.Parameters.AddWithValue("@Password", "");
                    cmd1.CommandTimeout = 6000;
                    cmd1.ExecuteNonQuery();

                    SqlDataAdapter da = new SqlDataAdapter(cmd1);
                    ds1.Clear();
                    da.Fill(ds1);

                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        StateMod.Text = ds1.Tables[0].Rows[0]["StateName"].ToString();
                        AreaMod.Text = ds1.Tables[0].Rows[0]["AreaName"].ToString();
                        ZoneMod.Text = ds1.Tables[0].Rows[0]["ZoneName"].ToString();
                        UserTypeMod.Text = ds1.Tables[0].Rows[0]["User_Type"].ToString();
                        UserIdMod.Text = ds1.Tables[0].Rows[0]["UserId"].ToString();
                        NameMod.Text = ds1.Tables[0].Rows[0]["UserName"].ToString();
                        MailMod.Text = ds1.Tables[0].Rows[0]["UserEmail"].ToString();
                        MobMod.Text = ds1.Tables[0].Rows[0]["UserMobileNo"].ToString();
                        PassMod.Text = ds1.Tables[0].Rows[0]["Password"].ToString();
                    }
                    con.Close();
                }
                catch (Exception ex)
                {
                    showToast("An error occurred: " + ex.Message, "toast-danger");
                }
            }
            else if (btn.ID == "ConfirmNo")
            {
                // Hide ConfirmDiv
                HtmlGenericControl confirmDiv = (HtmlGenericControl)FindControl("ConfirmDiv");
                if (confirmDiv != null)
                    confirmDiv.Style["display"] = "none";
            }
        }
        #endregion

        #region BtnModify_Click
        protected void BtnModify_Click(object sender, EventArgs e)
        {
            string mob = MobMod.Text;

            //if (string.IsNullOrEmpty(mob))
            //{
            //    showToast("Mobile No. should not be empty", "toast-danger");
            //    return;
            //}
            //else if (!Regex.IsMatch(mob, @"^\d+$"))
            //{
            //    showToast("Mobile No. should be numeric", "toast-danger");
            //    return;
            //}
            //else if (mob.Length > 10)
            //{
            //    showToast("Mobile No. should not be more than 10 digits", "toast-danger");
            //    return;
            //}
            //else if (mob.Length < 10)
            //{
            //    showToast("Mobile No. should not be less than 10 digits", "toast-danger");
            //    return;
            //}

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
            cmd1.Parameters.AddWithValue("@ActionType", "Modify");
            cmd1.Parameters.AddWithValue("@StateID", "");
            cmd1.Parameters.AddWithValue("@AreaID", "");
            cmd1.Parameters.AddWithValue("@ZoneID", "");
            cmd1.Parameters.AddWithValue("@UserType", UserTypeMod.Text);
            cmd1.Parameters.AddWithValue("@UserId", UserIdMod.Text);
            cmd1.Parameters.AddWithValue("@UserName", NameMod.Text);
            cmd1.Parameters.AddWithValue("@UserEmail", MailMod.Text);
            cmd1.Parameters.AddWithValue("@UserMobile", mob);
            cmd1.Parameters.AddWithValue("@Password", PassMod.Text);
            cmd1.CommandTimeout = 6000;
            cmd1.ExecuteNonQuery();

            showToast("Data Modified", "toast-success");

            // Show CreateDiv and Hide EditDiv
            HtmlGenericControl createDiv = (HtmlGenericControl)FindControl("CreateDiv");
            if (createDiv != null)
                createDiv.Style["display"] = "block";
            HtmlGenericControl editDiv = (HtmlGenericControl)FindControl("EditDiv");
            if (editDiv != null)
                editDiv.Style["display"] = "none";

            ClearForm();
        }
        #endregion

        #region ToastNotification
        private void showToast(string message, string styleClass)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showToast", $"showToast('{message}', '{styleClass}');", true);
        }




        #endregion

        #region Helpers
        protected void CPGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the user's role from AccessLoad
                string userRole = Session["Role"].ToString();

                // Get the value of "Selected" column
                //int selected = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Selected"));

                // Find the placeholder for the button
                PlaceHolder ph = (PlaceHolder)e.Row.FindControl("ButtonPlaceHolder");

                Button editBtn = (Button)ph.FindControl("EditBtn");
                Button createBtn = (Button)ph.FindControl("CreateBtn");
                TextBox existsBox = (TextBox)ph.FindControl("ExitsBox");
                TextBox createBox = (TextBox)ph.FindControl("CreateBox");

                if (userRole == "ADMIN")
                {
                    editBtn.Visible = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Selected")) == 1;
                    createBtn.Visible = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Selected")) == 0;
                    existsBox.Visible = false;
                    createBox.Visible = false;
                }
                else
                {
                    existsBox.Visible = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Selected")) == 1;
                    createBox.Visible = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "Selected")) == 0;
                    editBtn.Visible = false;
                    createBtn.Visible = false;
                }
            }
        }

        protected void EditBtn_Click(object sender, EventArgs e)
        {
            Button createBtn = (Button)sender;

            GridViewRow row = (GridViewRow)createBtn.NamingContainer;

            string userId = ((Label)row.FindControl("UserIdLabel")).Text;
            string userType = UserTypeDrp.SelectedValue;

            // Show EditDiv
            HtmlGenericControl editDiv = (HtmlGenericControl)FindControl("EditDiv");
            if (editDiv != null)
                editDiv.Style["display"] = "block";

            // Hide ConfirmDiv and CreateDiv
            HtmlGenericControl confirmDiv = (HtmlGenericControl)FindControl("ConfirmDiv");
            if (confirmDiv != null)
                confirmDiv.Style["display"] = "none";
            HtmlGenericControl createDiv = (HtmlGenericControl)FindControl("CreateDiv");
            if (createDiv != null)
                createDiv.Style["display"] = "none";

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd1 = new SqlCommand("SP_ClaimPortal_UserCreation_Dropdowns", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@session_Name", Session["name"].ToString());
                cmd1.Parameters.AddWithValue("@ActionType", "ModifyLoad");
                cmd1.Parameters.AddWithValue("@StateID", "");
                cmd1.Parameters.AddWithValue("@AreaID", "");
                cmd1.Parameters.AddWithValue("@ZoneID", "");
                cmd1.Parameters.AddWithValue("@UserType", userType);
                cmd1.Parameters.AddWithValue("@UserId", userId);
                cmd1.Parameters.AddWithValue("@UserName", "");
                cmd1.Parameters.AddWithValue("@UserEmail", "");
                cmd1.Parameters.AddWithValue("@UserMobile", "");
                cmd1.Parameters.AddWithValue("@Password", "");
                cmd1.CommandTimeout = 6000;
                cmd1.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(cmd1);
                ds1.Clear();
                da.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 0)
                {
                    StateMod.Text = ds1.Tables[0].Rows[0]["StateName"].ToString();
                    AreaMod.Text = ds1.Tables[0].Rows[0]["AreaName"].ToString();
                    ZoneMod.Text = ds1.Tables[0].Rows[0]["ZoneName"].ToString();
                    UserTypeMod.Text = ds1.Tables[0].Rows[0]["User_Type"].ToString();
                    UserIdMod.Text = ds1.Tables[0].Rows[0]["UserId"].ToString();
                    NameMod.Text = ds1.Tables[0].Rows[0]["UserName"].ToString();
                    MailMod.Text = ds1.Tables[0].Rows[0]["UserEmail"].ToString();
                    MobMod.Text = ds1.Tables[0].Rows[0]["UserMobileNo"].ToString();
                    PassMod.Text = ds1.Tables[0].Rows[0]["Password"].ToString();
                }
                con.Close();

                CPGridView.DataSource = null;
                CPGridView.DataBind();
            }
            catch (Exception ex)
            {
                showToast("An error occurred: " + ex.Message, "toast-danger");
            }
        }

        protected void CreateBtn_Click(object sender, EventArgs e)
        {
            Button createBtn = (Button)sender;

            GridViewRow row = (GridViewRow)createBtn.NamingContainer;

            string userId = ((Label)row.FindControl("UserIdLabel")).Text;

            UserId.Text = userId;
            CPGridView.DataSource = null;
            CPGridView.DataBind();
        }

        protected string MaskPassword(string password)
        {
            return new string('●', password.Length);
        }
        #endregion

        #region ClearForm
        public void ClearForm()
        {
            StateDrp.ClearSelection();
            AreaDrp.ClearSelection();
            ZoneDrp.ClearSelection();
            UserTypeDrp.ClearSelection();
            UserId.Text = string.Empty;
            UserName.Text = string.Empty;
            UserEmail.Text = string.Empty;
            UserMob.Text = string.Empty;
            UserPass.Text = string.Empty;
        }
        #endregion
    }
}