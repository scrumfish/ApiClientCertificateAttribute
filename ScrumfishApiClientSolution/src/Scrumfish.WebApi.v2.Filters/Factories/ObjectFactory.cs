using System;
using Microsoft.Azure;
using Scrumfish.WebApi.v2.Objects;
using Scrumfish.WebApi.v2.Objects.Interfaces;

namespace Scrumfish.WebApi.v2.Filters.Factories
{
    public static class ObjectFactory
    {
        public static ICertificateAuthenticationProvider CreateAuthenticationProvider()
        {
            var classInfo = CloudConfigurationManager.GetSetting("scrumfish.WebApi.v2.AuthenticationProvder");
            if (string.IsNullOrWhiteSpace(classInfo))
            {
                throw new MissingApiConfigEntryException("scrumfish.WebApi.v2.AuthenticationProvder is not set in the config.");
            }
            
            var instanceType = Type.GetType(classInfo);
            if (instanceType == null)
            {
                throw new  NullReferenceException("Type reference of scrumfish.WebApi.v2.AuthenticationProvider is null.");
            }
            var result = Activator.CreateInstance(instanceType) as ICertificateAuthenticationProvider;

            if (result == null)
            {
                throw new NullReferenceException("Could not create type from scrumfish.WebApi.V2.AuthetncationProvider.");
            }
            return result;
        }
    }
}
