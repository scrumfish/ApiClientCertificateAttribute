using System;
using System.Reflection;

namespace Scrumfish.WebApi.v2.ApiSite.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}