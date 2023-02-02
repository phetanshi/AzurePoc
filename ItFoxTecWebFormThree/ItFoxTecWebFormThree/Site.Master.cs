using ItFoxTecWebFormThree.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using System.Configuration;
using System.Web.Configuration;

namespace ItFoxTecWebFormThree
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Session["Reset"] = true;
            //Configuration config = WebConfigurationManager.OpenWebConfiguration("~/Web.Config");
            //SessionStateSection section = (SessionStateSection)config.GetSection("system.web/sessionState");
            SessionStateSection section = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");
            int timeout = (int)section.Timeout.TotalMinutes * 1000; //I change the timeout value to 25 seconds
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SessionAlert", "SessionExpireAlert(" + 3000 + ");", true);


            if (!IsPostBack)
            {
                if (Request.IsAuthenticated)
                {
                    var displayNameClaim = ClaimsPrincipal.Current.FindFirst(SamlClaimTypes.DisplayName);
                    LoginUserName.InnerText = displayNameClaim != null ? displayNameClaim.Value : "<No Name>";
                }
                else
                {
                    Response.Redirect("/Auth/Login");
                }
            }
        }
    }
}