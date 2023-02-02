
using System;
using System.Web;
using http = ITfoxtec.Identity.Saml2.Http;

namespace ItFoxTecWebFormThree.App_Start
{
    public static class AppExtensions
    {
        public static http.HttpRequest ToGenericHttpRequest2(this HttpRequestBase request, bool readBodyAsString = false)
        {

            http.HttpRequest httpRequest = new http.HttpRequest
            {
                Method = "POST",
                QueryString = request.Url.Query,
                Query = request.QueryString
            };

            string samlResp = request.QueryString["SAMLResponse"];

            httpRequest.Form = new System.Collections.Specialized.NameValueCollection();

            httpRequest.Form.Add("SAMLResponse", samlResp);
            return httpRequest;
        }
    }
}