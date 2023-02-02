using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Security.Claims;
using ItFoxTecWebFormThree.Identity;
using System.IdentityModel.Services;
using System.Security.Authentication;
using ItFoxTecWebFormThree.App_Start;
using System.Web.UI;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Text;

namespace ItFoxTecWebFormThree.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        const string relayStateReturnUrl = "ReturnUrl";
        private readonly Saml2Configuration config;

        protected XmlNamespaceManager _xmlNameSpaceManager;
        public AuthController()
        {
            config = IdentityConfig.Saml2Configuration;
        }
        public ActionResult Login(string returnUrl = null)
        {
            if(returnUrl == null)
            {
                returnUrl = GetStartupPage();
            }
            var binding = new Saml2RedirectBinding();
            var test = binding.SetRelayStateQuery(new Dictionary<string, string> { { relayStateReturnUrl, returnUrl ?? Url.Content("~/") } });

            return binding.Bind(new Saml2AuthnRequest(config)).ToActionResult();
        }

        private string GetStartupPage()
        {
            return "~/Default.aspx";
        }
        private static byte[] StringToByteArray(string st)
        {
            byte[] bytes = new byte[st.Length];
            for (int i = 0; i < st.Length; i++)
            {
                bytes[i] = (byte)st[i];
            }
            return bytes;
        }
        private static X509Certificate2 GetCertificate()
        {
            string samlCertificate = "-----BEGIN CERTIFICATE-----\r\nMIIC8DCCAdigAwIBAgIQGsdN/nkEJ7BGXeVkDxBjBzANBgkqhkiG9w0BAQsFADA0MTIwMAYDVQQD\r\nEylNaWNyb3NvZnQgQXp1cmUgRmVkZXJhdGVkIFNTTyBDZXJ0aWZpY2F0ZTAeFw0yMjExMjgwODQ3\r\nMzRaFw0yNTExMjgwODQ3MzNaMDQxMjAwBgNVBAMTKU1pY3Jvc29mdCBBenVyZSBGZWRlcmF0ZWQg\r\nU1NPIENlcnRpZmljYXRlMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAzwOa73MRTOf/\r\nGiHj3UQXluE0LSHHkoV77K8A7weqvnD/R6ZXn0eNCyvijUmCwVbmxsqQp0QsBAqClX7yakE/tqCt\r\noYIK/WWlpWEJoXXGSQZhcHZ2IAmSS2MGQN+Bqtd86vbupiHhn9636v4zig7stHCDdJ2qE26BNNPw\r\nUKKx7o6NGC+cW2bS9kAWZdCImjTfRVQOTsSuXXN63D6oPX1JgzuXWGD2PY2m3QRLZqP9Gww+R8YD\r\nq4P3qS79N49ZFdA+IynA/oEp3bSP2+LFniat19lBtyT6UE6Z07+cvlA1e94CwDXhQsDTb2uqLcTg\r\nPNzKiOsn4dXSzb1Y/41qSg8OEQIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQCV/aO6wOTXveA3+R8k\r\nmDzjj+eelvMm9Hnra1rkJQ/tnDkYSKoEeEueHRr0q0jxdZI+oDNY7Z0iq/pYxDKCY63dwi6tLoDM\r\nVlKWmJ9Cvv4vbMEt9+gARhu3PknPiq7UjjTEtPT8bFI3r5nme5116KTrCPKs86CD4gjv9OlghHcb\r\nQtFCmev4uqcRK4V7BjSaE6O/oW2aqGw7K295qftQeBQwzgjUR9pbREUzckp0V4v6axU7ewt/pDqi\r\n0AIEvx+FRSxdO+x3J8Ij4q1t+dq9bPwcs12Q2fWLiIgBio7+/uDLs0wY7ZZx/cUh7mfRFxXslitT\r\nEvxZylZN+5wByaJFVddL\r\n-----END CERTIFICATE-----\r\n";

            var cert = StringToByteArray(samlCertificate);

            return new X509Certificate2(cert);
        }

        private XmlNamespaceManager GetNamespaceManager(XmlDocument xmlDoc)
        {
            XmlNamespaceManager manager = new XmlNamespaceManager(xmlDoc.NameTable);
            manager.AddNamespace("ds", SignedXml.XmlDsigNamespaceUrl);
            manager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
            manager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

            return manager;
        }

        private bool VerifyXml(XmlDocument Doc)
        {
            if (Doc == null)
                throw new ArgumentException("Doc");
            SignedXml signedXml = new SignedXml(Doc);
            var nsManager = new XmlNamespaceManager(Doc.NameTable);
            nsManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            var node = Doc.SelectSingleNode("//ds:Signature", nsManager);
            // find signature node
            var certElement = Doc.SelectSingleNode("//ds:X509Certificate", nsManager);
            // find certificate node
            var cert = new X509Certificate2(Convert.FromBase64String(certElement.InnerText));
            signedXml.LoadXml((XmlElement)node);
            return signedXml.CheckSignature(cert, true);
        }

        public XmlDocument LoadXml(string xml)
        {
            XmlDocument _xmlDoc = new XmlDocument();
            _xmlDoc.PreserveWhitespace = true;
            _xmlDoc.XmlResolver = null;
            _xmlDoc.LoadXml(xml);

            _xmlNameSpaceManager = GetNamespaceManager(_xmlDoc); //lets construct a "manager" for XPath queries

            return _xmlDoc;
        }

        public XmlDocument LoadXmlFromBase64(string response)
        {
            UTF8Encoding enc = new UTF8Encoding();
            XmlDocument xDoc = LoadXml(enc.GetString(Convert.FromBase64String(response)));

            return xDoc;
        }

        public ActionResult AssertionConsumerService()
        {
            var binding = new Saml2PostBinding();
            var saml2AuthnResponse = new Saml2AuthnResponse(config);
            var samlResponse = Request.Form["SAMLResponse"];

            XmlDocument xDoc = LoadXmlFromBase64(samlResponse);

            bool isValid = VerifyXml(xDoc);

            if (!isValid)
                return null;

            binding.ReadSamlResponse(Request.ToGenericHttpRequest(), saml2AuthnResponse);
            if (saml2AuthnResponse.Status != Saml2StatusCodes.Success)
            {
                throw new AuthenticationException($"SAML Response status: {saml2AuthnResponse.Status}");
            }
            binding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnResponse);
            saml2AuthnResponse.CreateSession(claimsAuthenticationManager: new DefaultClaimsAuthenticationManager());

            var relayStateQuery = binding.GetRelayStateQuery();
            var returnUrl = relayStateQuery.ContainsKey(relayStateReturnUrl) ? relayStateQuery[relayStateReturnUrl] : Url.Content("~/");
            return Redirect(returnUrl);
        }

        //[ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect(Url.Content("~/"));
            }

            var binding = new Saml2PostBinding();
            var logoutRequest = new Saml2LogoutRequest(config, ClaimsPrincipal.Current).DeleteSession();
            return binding.Bind(logoutRequest).ToActionResult();
        }

        public ActionResult LoggedOut()
        {
            var binding = new Saml2PostBinding();
            var logoutResp = new Saml2LogoutResponse(config);
            var httpReq = Request.ToGenericHttpRequest2();
            binding.Unbind(httpReq, logoutResp);

            FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            return Redirect(Url.Content("~/logout"));
        }

        [HttpPost]
        public ActionResult TestLogout()
        {
            var binding = new Saml2PostBinding();
            var logoutResp = new Saml2LogoutResponse(config);
            var httpReq = Request.ToGenericHttpRequest();
            binding.Unbind(httpReq, logoutResp);

            FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            return Redirect(Url.Content("~/logout"));
        }
        public ActionResult SingleLogout()
        {
            Saml2StatusCodes status;
            var requestBinding = new Saml2PostBinding();
            var logoutRequest = new Saml2LogoutRequest(config, ClaimsPrincipal.Current);
            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), logoutRequest);
                status = Saml2StatusCodes.Success;
                logoutRequest.DeleteSession();
            }
            catch (Exception exc)
            {
                // log exception
                Debug.WriteLine("SingleLogout error: " + exc.ToString());
                status = Saml2StatusCodes.RequestDenied;
            }

            var responsebinding = new Saml2PostBinding();
            responsebinding.RelayState = requestBinding.RelayState;
            var saml2LogoutResponse = new Saml2LogoutResponse(config)
            {
                InResponseToAsString = logoutRequest.IdAsString,
                Status = status,
            };
            return responsebinding.Bind(saml2LogoutResponse).ToActionResult();
        }
    }
}