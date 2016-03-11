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
            bool fullValidation;
            bool.TryParse(CloudConfigurationManager.GetSetting("scrumfish.SimpleProvider.FullValidation"),
                out fullValidation);
            if (fullValidation)
            {
                if (!certificate.Verify())
                {
                    return false;
                }
            }
            else
            {
                if (certificate.NotAfter < DateTime.UtcNow || certificate.NotBefore > DateTime.UtcNow)
                {
                    return false;
                }
            }
            var users = GetUsers();
            var certs = users
                .Where(u => ! string.IsNullOrWhiteSpace(u.Certificate))
                .Select(u => (new X509Certificate2(Encoding.UTF8.GetBytes(u.Certificate)).Thumbprint));
            return certs.Any(c => c.Equals(certificate.Thumbprint,StringComparison.InvariantCultureIgnoreCase));
        }

        public IPrincipal GetPrincipal(X509Certificate2 certificate)
        {
            throw new NotImplementedException();
        }

        public IPrincipal GetUnsecuredUserForTestingOnly()
        {
            var userEnties = GetUsers();
            var testUserName = CloudConfigurationManager.GetSetting("scrumfish.SimpleProvider.TestUser");
            var userEntry = userEnties.FirstOrDefault(u => string.IsNullOrWhiteSpace(testUserName) || u.UserName == testUserName);
            if (userEntry == null)
            {
                return null;
            }
            var claims = (from ClaimElement claim in userEntry.Claims
                          select new Claim(claim.Claim, claim.Value))
                .ToList();
            if (claims.All(c => c.Type != "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"))
            {
                claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", userEntry.UserName));
            }
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        private static IEnumerable<ConfigUserElement> GetUsers()
        {
            var config = SimpleProviderSection.GetProviderSection();
            return (from ConfigUserElement user in config.Users
                select user);
        }
    }
}
