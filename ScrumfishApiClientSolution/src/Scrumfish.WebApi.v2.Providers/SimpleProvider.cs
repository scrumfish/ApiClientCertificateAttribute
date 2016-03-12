using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using Microsoft.Azure;
using Scrumfish.WebApi.v2.Objects.Config;
using Scrumfish.WebApi.v2.Objects.Interfaces;

namespace Scrumfish.WebApi.v2.Providers
{
    public class SimpleProvider : ICertificateAuthenticationProvider
    {
        public bool Validate(X509Certificate2 certificate)
        {
            ValidationMode validationMode;
            if (!Enum.TryParse(CloudConfigurationManager.GetSetting("scrumfish.SimpleProvider.ValidationMode"),
                out validationMode))
            {
                validationMode = ValidationMode.Full;
            }

            switch (validationMode)
            {
                case ValidationMode.Full:
                    if (!certificate.Verify())
                    {
                        return false;
                    }
                    break;
                case ValidationMode.DatesOnly:
                    if (certificate.NotAfter < DateTime.UtcNow || certificate.NotBefore > DateTime.UtcNow)
                    {
                        return false;
                    }
                    break;
                case ValidationMode.None:
                default:
                    break;
            }

            var users = GetUsers();
            var certs = users
                .Select(u =>
                    string.IsNullOrWhiteSpace(u.Certificate)
                    ? u.Thumbprint
                    : (new X509Certificate2(Encoding.UTF8.GetBytes(u.Certificate)).Thumbprint));
            return certs.Any(c => c.Equals(certificate.Thumbprint, StringComparison.InvariantCultureIgnoreCase));
        }

        public IPrincipal GetPrincipal(X509Certificate2 certificate)
        {
            var userEntry = GetUsers()
                            .FirstOrDefault(u =>
                            {
                                var s = string.IsNullOrWhiteSpace(u.Certificate)
                                    ? u.Thumbprint
                                    : (new X509Certificate2(Encoding.UTF8.GetBytes(u.Certificate)).Thumbprint);
                                return s != null && s.Equals(
                                            certificate.Thumbprint, StringComparison.InvariantCultureIgnoreCase);
                            });
            return userEntry == null ? null : new ClaimsPrincipal(new ClaimsIdentity(GatherClaims(userEntry)));
        }

        public IPrincipal GetUnsecuredUserForTestingOnly()
        {
            var testUserName = CloudConfigurationManager.GetSetting("scrumfish.SimpleProvider.TestUser");
            var userEntry = GetUsers()
                            .FirstOrDefault(u => string.IsNullOrWhiteSpace(testUserName) || u.UserName == testUserName);
            return userEntry == null ? null : new ClaimsPrincipal(new ClaimsIdentity(GatherClaims(userEntry)));
        }

        private static List<Claim> GatherClaims(ConfigUserElement userEntry)
        {
            var claims = (from ClaimElement claim in userEntry.Claims
                          select new Claim(claim.Claim, claim.Value))
                .ToList();
            if (claims.All(c => c.Type != "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"))
            {
                claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", userEntry.UserName));
            }
            return claims;
        }

        private static IEnumerable<ConfigUserElement> GetUsers()
        {
            var config = SimpleProviderSection.GetProviderSection();
            return (from ConfigUserElement user in config.Users
                    select user);
        }
    }
}
