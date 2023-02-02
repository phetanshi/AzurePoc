using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PS.ConsoleApp.Application
{
    public enum AuthRequestFormat
    {
        Base64 = 1
    }
    public class SamlGen
    {
        const string FILENAME = @"E:\Git\POCs\AzureAd\test.xml";
        public string _id;
        public string _assertion_id;
        private string _issue_instant;

        private string _issuer;
        private string _assertionConsumerServiceUrl;
        private string _acsUrl = "https://localhost:44300/Auth/AssertionConsumerService";

        public SamlGen()
        {
            _id = "_" + Guid.NewGuid().ToString();
            _assertion_id = "_" + Guid.NewGuid().ToString();
            _issue_instant = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", System.Globalization.CultureInfo.InvariantCulture);
        }
        
        public string GenerateSamlResponse()
        {
            using (StringWriter sw = new StringWriter())
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.OmitXmlDeclaration = true;
                using (XmlWriter xw = XmlWriter.Create(sw, xws))
                {
                    xw.WriteStartElement("samlp", "Response", "urn:oasis:names:tc:SAML:2.0:protocol");
                    xw.WriteAttributeString("ID", _id);
                    xw.WriteAttributeString("Version", "2.0");
                    xw.WriteAttributeString("IssueInstant", _issue_instant);
                    xw.WriteAttributeString("Destination", "https://localhost:44300/Auth/AssertionConsumerService");
                    xw.WriteAttributeString("InResponseTo", _id);
                    xw.GetIssuer(true);
                    xw.GetStatusElement();
                    xw.GetAssertion(_assertion_id, _issue_instant);

                    xw.WriteEndElement();
                }

                var memoryStream = new MemoryStream();
                var writer = new StreamWriter(new DeflateStream(memoryStream, CompressionMode.Compress, true), new UTF8Encoding(false));
                writer.Write(sw.ToString());
                writer.Close();
                string result = Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length, Base64FormattingOptions.None);
                return result;
            }
        }

        

        public void SignXmlWithCertificate(XmlElement assertion, X509Certificate2 cert)
        {
            SignedXml signedXml = new SignedXml(assertion)
            {
                SigningKey = cert.PrivateKey
            };
            Reference reference = new Reference();
            reference.Uri = "";
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);

            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));

            signedXml.KeyInfo = keyInfo;
            signedXml.ComputeSignature();
            XmlElement xmlsig = signedXml.GetXml();

            assertion.AppendChild(xmlsig);
        }
        
    }
}
