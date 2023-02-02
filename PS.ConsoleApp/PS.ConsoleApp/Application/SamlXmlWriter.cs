using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PS.ConsoleApp.Application
{
    public static class SamlXmlWriter
    {
        public static string _id = "_" + Guid.NewGuid().ToString();
        public static string _acsUrl = "https://localhost:44300/Auth/AssertionConsumerService";
        public const string _claimBaseUrl = "http://schemas.microsoft.com/identity/claims";
        public static void GetStatusElement(this XmlWriter xw)
        {
            xw.WriteStartElement("samlp", "Status", null);
            xw.WriteStartElement("samlp", "StatusCode", null);
            xw.WriteAttributeString("Value", "urn:oasis:names:tc:SAML:2.0:status:Success");
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        public static void GetAssertion(this XmlWriter xw, string assertionId, string issueInstance)
        {
            xw.WriteStartElement("Assertion", "urn:oasis:names:tc:SAML:2.0:assertion");
            xw.WriteAttributeString("ID", assertionId);
            xw.WriteAttributeString("IssueInstant", issueInstance);
            xw.WriteAttributeString("Version", "2.0");
            xw.GetIssuer();
            xw.GetSubject();
            xw.GetConditions();
            xw.GetAttributeStatement();
            xw.GetAuthnStatement();
            xw.WriteEndElement();
        }

        public static void GetIssuer(this XmlWriter xw, bool includeNameSpace = false)
        {
            
            if(includeNameSpace)
            {
                xw.WriteStartElement("Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
            }
            else
            {
                xw.WriteStartElement("Issuer");
            }

            xw.WriteString("https://sts.windows.net/945eb2a4-c583-475f-9019-c25b76b87436/");
            xw.WriteEndElement();
        }
        public static void GetSubject(this XmlWriter xw)
        {
            xw.WriteStartElement("Subject");
            xw.GetNameIdFormat();
            xw.GetSubjectConfirmation();
            xw.WriteEndElement();
        }

        public static void GetNameIdFormat(this XmlWriter xw)
        {
            xw.WriteStartElement("NameID");
            xw.WriteAttributeString("Format", "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress");
            xw.WriteString("padmasekhar@test.com");
            xw.WriteEndElement();
        }

        public static void GetSubjectConfirmation(this XmlWriter xw)
        {
            xw.WriteStartElement("SubjectConfirmation");
            xw.WriteAttributeString("Method", "urn:oasis:names:tc:SAML:2.0:cm:bearer");
            xw.GetSubjectConfirmationData();
            xw.WriteEndElement();
        }

        public static void GetSubjectConfirmationData(this XmlWriter xw)
        {
            xw.WriteStartElement("SubjectConfirmationData");
            xw.WriteAttributeString("InResponseTo", _id);
            xw.WriteAttributeString("NotOnOrAfter", "2024-02-01T07:27:16.726Z");
            xw.WriteAttributeString("Recipient", _acsUrl);
            xw.WriteEndElement();
        }

        public static void GetConditions(this XmlWriter xw)
        {
            xw.WriteStartElement("Conditions");
            xw.WriteAttributeString("NotBefore", "2023-02-01T07:27:16.726Z");
            xw.WriteAttributeString("NotOnOrAfter", "2024-02-01T07:27:16.726Z");
            xw.GetAudienceRestriction();

            xw.WriteEndElement();
        }

        public static void GetAudienceRestriction(this XmlWriter xw)
        {
            xw.WriteStartElement("AudienceRestriction");
            xw.GetAudience();
            xw.WriteEndElement();
        }
        public static void GetAudience(this XmlWriter xw)
        {
            xw.WriteStartElement("Audience");
            xw.WriteString("https://localhost:44300/");
            xw.WriteEndElement();
        }

        public static void GetAttributeStatement(this XmlWriter xw)
        {
            xw.WriteStartElement("AttributeStatement");
            xw.GetAttribute("http://schemas.microsoft.com/identity/claims/tenantid");
            xw.GetAttribute("http://schemas.microsoft.com/identity/claims/objectidentifier");
            xw.GetAttribute("http://schemas.microsoft.com/identity/claims/displayname");
            xw.GetAttribute("http://schemas.microsoft.com/identity/claims/identityprovider");

            xw.GetAttribute("http://schemas.microsoft.com/claims/authnmethodsreferences",
                            "http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/password",
                            "http://schemas.microsoft.com/claims/multipleauthn",
                            "http://schemas.microsoft.com/ws/2008/06/identity/authenticationmethod/unspecified");

            xw.GetAttribute("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            xw.GetAttribute("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");
            xw.GetAttribute("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            xw.WriteEndElement();
        }

        private static void GetAttribute(this XmlWriter xw, string claim)
        {
            string value = GetClaimValue(claim);
            xw.WriteStartElement("Attribute");
            xw.WriteAttributeString("Name", claim);
            xw.WriteStartElement("AttributeValue");
            xw.WriteString(value);
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        private static string GetClaimValue(string claim)
        {
            string value = "";
            AppClaim appClaim = new AppClaim();
            switch (claim)
            {
                case AppClaimTypes.TenantId:
                    value = appClaim.GetTenantId();
                    break;
                case AppClaimTypes.ObjectIdentifier:
                    value = appClaim.GetObjectIdentifier();
                    break;
                case AppClaimTypes.DisplayName:
                    value = appClaim.GetDisplayName();
                    break;
                case AppClaimTypes.IdentityProvider:
                    value = appClaim.GetIdentityProvider();
                    break;
                case ClaimTypes.GivenName:
                    value = appClaim.GetGivenName();
                    break;
                case ClaimTypes.Surname:
                    value = appClaim.GetSurname();
                    break;
                case ClaimTypes.Email:
                    value = appClaim.GetEmail();
                    break;
                case ClaimTypes.Name:
                    value = appClaim.GetName();
                    break;
            }
            return value;
        }

        private static void GetAttribute(this XmlWriter xw, string name, params string[] values)
        {
            xw.WriteStartElement("Attribute");
            xw.WriteAttributeString("Name", name);
            
            foreach(string val in values)
            {
                xw.WriteStartElement("AttributeValue");
                xw.WriteString(val);
                xw.WriteEndElement();
            }

            xw.WriteEndElement();
        }

        public static void GetAuthnStatement(this XmlWriter xw)
        {
            xw.WriteStartElement("AuthnStatement");
            xw.WriteAttributeString("AuthnInstant", DateTime.UtcNow.ToString("s") + "Z");
            xw.WriteAttributeString("SessionIndex", "_db0c9fc7-c3ef-4e53-9790-2293e1b37b00");
            xw.GetAuthnContext();
            xw.WriteEndElement();
        }

        public static void GetAuthnContext(this XmlWriter xw)
        {
            xw.WriteStartElement("AuthnContext");
            xw.GetAuthnContextClassRef();
            xw.WriteEndElement();
        }
        public static void GetAuthnContextClassRef(this XmlWriter xw)
        {
            xw.WriteStartElement("AuthnContextClassRef");
            xw.WriteString("urn:oasis:names:tc:SAML:2.0:ac:classes:Password");
            xw.WriteEndElement();
        }
    }
}
