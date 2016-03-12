using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Scrumfish.WebApi.v2.Objects.Interfaces;

namespace Scrumfish.WebApi.v2.Filters.Tests
{
    [TestClass]
    public class ClientCertificateAuthenticateFilter_Tests
    {
        private ClientCertificateAuthenticateFilter _target;
        private Mock<ICertificateAuthenticationProvider> _certificateMock;
        private HttpAuthenticationContext _authenticationContext;
        private CancellationToken _cancellationToken;

        [TestInitialize]
        public void Setup()
        {
            _certificateMock = new Mock<ICertificateAuthenticationProvider>();

            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.Enabled"] = "true";
            _target = new ClientCertificateAuthenticateFilter();
            var authenticationProvider =
                typeof (ClientCertificateAuthenticateFilter).GetProperty("AuthenticationProvider",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            authenticationProvider.SetValue(_target, _certificateMock.Object);

            var request = new HttpRequestMessage();
            var controllerContext = new HttpControllerContext();
            controllerContext.Request = request;
            var context = new HttpActionContext();
            context.ControllerContext = controllerContext;
            _authenticationContext = new HttpAuthenticationContext(context, null);
            var headers = request.Headers;
            var authorization = new AuthenticationHeaderValue("scheme");
            headers.Authorization = authorization;
            _cancellationToken = new CancellationToken();
        }

        [TestMethod]
        public void AuthenticateAsync_ReturnsNullErrorResultIfConfigEnabledIsFalse_Test()
        {
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.Enabled"] = "false";
            var result = _target.AuthenticateAsync(_authenticationContext, _cancellationToken);
            result.Wait(_cancellationToken);
            Assert.IsNull(_authenticationContext.ErrorResult);
        }

        [TestMethod]
        public void AuthenticateAsync_ReturnsExpectedPrincipalIfConfigEnabledIsFalse_Test()
        {
            var expected = new ClaimsPrincipal();
            _certificateMock.Setup(cm => cm.GetUnsecuredUserForTestingOnly()).Returns(expected);
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.Enabled"] = "false";
            var result = _target.AuthenticateAsync(_authenticationContext, _cancellationToken);
            result.Wait(_cancellationToken);
            Assert.AreSame(expected, _authenticationContext.Principal);
        }

        [TestMethod]
        public void AuthenticateAsync_ReturnsExpectedPrincipalIfConfigCertificateIsValid_Test()
        {
            var expected = new ClaimsPrincipal();
            _certificateMock.Setup(cm => cm.GetPrincipal(It.IsAny<X509Certificate2>())).Returns(expected);
            _certificateMock.Setup(cm => cm.Validate(It.IsAny<X509Certificate2>())).Returns(true);
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.Enabled"] = "true";
            _authenticationContext.Request.Properties.Add(HttpPropertyKeys.ClientCertificateKey, new X509Certificate2());
            var result = _target.AuthenticateAsync(_authenticationContext, _cancellationToken);
            result.Wait(_cancellationToken);
            Assert.AreSame(expected, _authenticationContext.Principal);
        }

        [TestMethod]
        public void AuthenticateAsync_ReturnsExpectedNullPrincipalIfConfigCertificateIsNotValid_Test()
        {
            var expected = new ClaimsPrincipal();
            _certificateMock.Setup(cm => cm.GetPrincipal(It.IsAny<X509Certificate2>())).Returns(expected);
            _certificateMock.Setup(cm => cm.Validate(It.IsAny<X509Certificate2>())).Returns(false);
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.Enabled"] = "true";
            _authenticationContext.Request.Properties.Add(HttpPropertyKeys.ClientCertificateKey, new X509Certificate2());
            var result = _target.AuthenticateAsync(_authenticationContext, _cancellationToken);
            result.Wait(_cancellationToken);
            Assert.IsNull(_authenticationContext.Principal);
        }

        [TestMethod]
        public void AuthenticateAsync_ReturnsErrorIfConfigCertificateIsNotValid_Test()
        {
            var expected = new ClaimsPrincipal();
            _certificateMock.Setup(cm => cm.GetPrincipal(It.IsAny<X509Certificate2>())).Returns(expected);
            _certificateMock.Setup(cm => cm.Validate(It.IsAny<X509Certificate2>())).Returns(false);
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.Enabled"] = "true";
            _authenticationContext.Request.Properties.Add(HttpPropertyKeys.ClientCertificateKey, new X509Certificate2());
            var result = _target.AuthenticateAsync(_authenticationContext, _cancellationToken);
            result.Wait(_cancellationToken);
            Assert.IsNotNull(_authenticationContext.ErrorResult);
        }

        [TestMethod]
        public void AuthenticateAsync_ReturnsErrorIfConfigCertificateIsNull_Test()
        {
            var expected = new ClaimsPrincipal();
            _certificateMock.Setup(cm => cm.GetPrincipal(It.IsAny<X509Certificate2>())).Returns(expected);
            _certificateMock.Setup(cm => cm.Validate(It.IsAny<X509Certificate2>())).Returns(true);
            ConfigurationManager.AppSettings["scrumfish.WebApi.v2.Enabled"] = "true";
            var result = _target.AuthenticateAsync(_authenticationContext, _cancellationToken);
            result.Wait(_cancellationToken);
            Assert.IsNotNull(_authenticationContext.ErrorResult);
        }
    }
}
