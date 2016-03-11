using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scrumfish.WebApi.v2.Objects;

namespace Scrumfish.WebApi.v2.Filters.Tests
{
    [TestClass]
    public class ObjectFactory_Tests
    {
        [TestCleanup]
        public void Teardown()
        {
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.AuthenticationProvder"] = null;
        }

        [TestMethod]
        public void CreateAuthenticationProvider_ReturnsExpectedClass_Test()
        {
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.AuthenticationProvder"] =
                "Scrumfish.WebApi.v2.Filters.Tests.Mock.AuthenticateMock, Scrumfish.WebApi.v2.Filters.Tests.Mock";
            var result = Factories.ObjectFactory.CreateAuthenticationProvider();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof (MissingApiConfigEntryException))]
        public void CreateAuthenticationProvider_ThrowsExceptionIfEntryIsMissingOrBlank_Test()
        {
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.AuthenticationProvder"] = null;
            Factories.ObjectFactory.CreateAuthenticationProvider();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateAuthenticationProvider_ThrowsExceptionIfEntryCannotBeLoaded_Test()
        {
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.AuthenticationProvder"] = "not.a.real.assembly.class, not.a.real.assembly";
            Factories.ObjectFactory.CreateAuthenticationProvider();
        }
    }
}
