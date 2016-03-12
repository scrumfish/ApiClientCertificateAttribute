using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;

namespace Scrumfish.WebApi.v2.Objects.Interfaces
{
    public interface ICertificateAuthenticationProvider
    {
        bool Validate(X509Certificate2 certificate);
        IPrincipal GetPrincipal(X509Certificate2 certificate);
        IPrincipal GetUnsecuredUserForTestingOnly();
    }
}
