using System.Web;
using System.Web.Mvc;

namespace Scrumfish.WebApi.v2.ApiSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
