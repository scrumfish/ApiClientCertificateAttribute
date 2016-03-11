using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Scrumfish.WebApi.v2.Filters.Actions
{
    public class ChallengeUnauthorizedResult : IHttpActionResult
    {
        public IHttpActionResult InnerResult { get; set; }

        public AuthenticationHeaderValue Header { get; set; }

        public ChallengeUnauthorizedResult(AuthenticationHeaderValue header, IHttpActionResult innerResult)
        {
            Header = header;
            InnerResult = innerResult;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await InnerResult.ExecuteAsync(cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (response.Headers.WwwAuthenticate.All(h => h.Scheme != Header.Scheme))
                {
                    response.Headers.WwwAuthenticate.Add(Header);
                }
            }
            return response;
        }
    }
}
