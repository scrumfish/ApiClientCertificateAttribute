using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Scrumfish.WebApi.v2.Filters.Actions
{
    public class AuthenticateFailResult : IHttpActionResult
    {
        public HttpRequestMessage Request { get; set; }

        public string Reason { get; set; }

        public AuthenticateFailResult(string reason, HttpRequestMessage request)
        {
            Reason = reason;
            Request = request;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                RequestMessage = Request,
                ReasonPhrase = Reason
            };
            return response;
        }
    }
}
