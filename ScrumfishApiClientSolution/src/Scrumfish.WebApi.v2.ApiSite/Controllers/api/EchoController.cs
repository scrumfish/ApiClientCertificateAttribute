using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Scrumfish.WebApi.v2.ApiSite.Controllers.api
{
    public class EchoController : ApiController
    {
        [HttpGet]
        public string Get([FromUri]string message)
        {
            return message;
        }
    }
}
