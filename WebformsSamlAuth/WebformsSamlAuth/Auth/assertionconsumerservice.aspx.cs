using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebformsSamlAuth.Auth
{
    public partial class assertionconsumerservice : System.Web.UI.Page
    {
        public string UserName { get; set; }
        public string SamlValidationError { get; set; } = "No Error";
        protected void Page_Load(object sender, EventArgs e)
        {
            string samlCertificateFile = "E:\\Downloads\\AzureRugh\\saml_pnshaz.cer";
            string samlCertificate = File.ReadAllText(samlCertificateFile);

            var samlResponse = new Response(samlCertificate, Request.Form["SAMLResponse"]);

            // 3. We're done!
            if (samlResponse.IsValid())
            {
                //WOOHOO!!! user is logged in
                LabelSuccess.Text = samlResponse.GetNameID();
            }
            else
            {
                LabelErrorMessage.Text = "Saml response have been failed";
            }
        }
    }
}