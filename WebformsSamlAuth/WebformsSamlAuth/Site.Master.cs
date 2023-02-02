using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebformsSamlAuth.Auth;

namespace WebformsSamlAuth
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Request.IsAuthenticated)
            {
                string samlEndpoint = ConfigurationManager.AppSettings["Saml2:SingleSignOnDestination"];
                string issuer = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
                var assertionConsumerService = issuer + "auth/assertionconsumerservice";
                var request = new AuthRequest(issuer, assertionConsumerService);
                var login = request.GetRedirectUrl(samlEndpoint);
                Response.Redirect(login);
            }
        }
    }
}