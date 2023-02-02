using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PS.ConsoleApp.Application
{
    public static class AppClaimTypes
    {
        public const string Temp = "";
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";
        public const string ObjectIdentifier = "http://schemas.microsoft.com/identity/claims/objectidentifie";
        public const string DisplayName = "http://schemas.microsoft.com/identity/claims/displayname";
        public const string IdentityProvider = "http://schemas.microsoft.com/identity/claims/identityprovider";


    }
}
