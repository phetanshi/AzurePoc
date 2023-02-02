using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PS.ConsoleApp.Application
{
    internal class App
    {
        public void Run()
        {
            try
            {
                //ViewCertificates();
                SamlGen saml = new SamlGen();
                saml.GenerateSamlResponse();
                Console.WriteLine("Completed..");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.ToString()}");
            }
            
        }

        public void ViewCertificates()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 mCert in store.Certificates)
            {
                Console.WriteLine("\n------------------------------------------------------------------\n");
                Console.WriteLine("mCert.FriendlyName : {0}", mCert.FriendlyName);
                Console.WriteLine("mCert.Thumbprint : {0}", mCert.Thumbprint);
                Console.WriteLine("mCert.SerialNumber : {0}", mCert.SerialNumber);

                Console.WriteLine("mCert.GetExpirationDateString : {0}", mCert.GetExpirationDateString());
                Console.WriteLine("\n------------------------------------------------------------------\n");
            }
        }
    }
}
