using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Microsoft.Azure;
using Scrumfish.WebApi.v2.Filters.Actions;
using Scrumfish.WebApi.v2.Objects.Interfaces;

namespace Scrumfish.WebApi.v2.Filters
{
    public class ClientCertificateAuthenticateFilter : IAuthenticationFilter
    {
        private ICertificateAuthenticationProvider _authenticationProvider;

        protected ICertificateAuthenticationProvider AuthenticationProvider
        {
            get { return _authenticationProvider ?? (Factories.ObjectFactory.CreateAuthenticationProvider()); }
            set { _authenticationProvider = value; }
        }

        public bool AllowMultiple => true;

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var enabledValue = CloudConfigurationManager.GetSetting("scrumfish.WebApi.v2.Enabled") ?? "true";
            bool enabled;
            if (!bool.TryParse(enabledValue, out enabled))
            {
                enabled = true;
            }

            if (!enabled)
            {
                context.Principal = AuthenticationProvider.GetUnsecuredUserForTestingOnly();
                return Task.FromResult(0);
            }

            var certificate = context.Request.GetClientCertificate();
            if (certificate == null || !AuthenticationProvider.Validate(certificate))
            {
                context.ErrorResult =
                    new AuthenticateFailResult(
                        certificate == null ? "No certificate provided by caller." : "Certificate is invalid.",
                        context.Request);
            }
            else
            {
                context.Principal = AuthenticationProvider.GetPrincipal(certificate);
            }
            return Task.FromResult(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            var challenge = new AuthenticationHeaderValue("Transport");
            context.Result = new ChallengeUnauthorizedResult(challenge, context.Result);
            return Task.FromResult(0);
        }
    }
}
