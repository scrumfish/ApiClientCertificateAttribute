using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Scrumfish.WebApi.v2.Providers.Tests
{
    [TestClass]
    public class SimpleProvider_Tests
    {
        private SimpleProvider _target;

        [TestInitialize]
        public void Setup()
        {
            _target = new SimpleProvider();
        }


        [TestCleanup]
        public void TearDown()
        {
            ConfigurationManager.AppSettings["scrumfish.SimpleProvider.TestUser"] = null;
            ConfigurationManager.AppSettings["scrumfish.SimpleProvider.ValidationMode"] = "None";
        }

        private static X509Certificate2 GetX509Certificate(string fileName)
        {
            var certificate = new X509Certificate2(fileName);
            return certificate;
        }

        [TestMethod]
        public void GetUnsecuredUserForTestingOnly_ReturnsFirstUserName_Test()
        {
            var expected = "Expired User";
            var result = _target.GetUnsecuredUserForTestingOnly();
            Assert.AreEqual(expected, result.Identity.Name);
        }

        [TestMethod]
        public void GetUnsecuredUserForTestingOnly_ReturnsCorrectClaimCount_Test()
        {
            var expected = 5;
            var result = _target.GetUnsecuredUserForTestingOnly() as ClaimsPrincipal;
            Assert.AreEqual(expected, result.Claims.Count());
        }

        [TestMethod]
        public void GetUnsecuredUserForTestingOnly_ReturnsExpectedClaimsUris_Test()
        {
            var expected = new[]
            {
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            };
            var result = _target.GetUnsecuredUserForTestingOnly() as ClaimsPrincipal;
            CollectionAssert.AreEquivalent(expected,result.Claims.Select(c => c.Type).ToList());
        }

        [TestMethod]
        public void GetUnsecuredUserForTestingOnly_ReturnsExpectedClaimsValues_Test()
        {
            var expected = new[]
            {
                "Expired User",
                "expired.user@example.com",
                "userfi001@domain.example.com",
                "User",
                "PowerUser"
            };
            var result = _target.GetUnsecuredUserForTestingOnly() as ClaimsPrincipal;
            CollectionAssert.AreEquivalent(expected, result.Claims.Select(c => c.Value).ToList());
        }

        [TestMethod]
        public void GetUnsecuredUserForTestingOnly_ReturnsExpectedTestUser_Test()
        {
            var expected = "Valid User";
            ConfigurationManager.AppSettings["scrumfish.SimpleProvider.TestUser"] = expected;
            var result = _target.GetUnsecuredUserForTestingOnly();
            Assert.AreEqual(expected, result.Identity.Name);
        }

        [TestMethod]
        public void Validate_ReturnsTrueIfCertificateFoundAndValid_Test()
        {
            var certificate = GetX509Certificate("valid.example.com.cer");
            var result = _target.Validate(certificate);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Validate_ReturnsFalseIfCertificateFoundAndExpired_Test()
        {
            ConfigurationManager.AppSettings["scrumfish.SimpleProvider.ValidationMode"] = "DatesOnly";
            var certificate = GetX509Certificate("expired.example.com.cer");
            var result = _target.Validate(certificate);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Validate_ReturnsFalseIfCertificateNotFound_Test()
        {
            var certificate = GetX509Certificate("unused.example.com.cer");
            var result = _target.Validate(certificate);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Validate_ReturnsTrueIfCertificateThumbprintFoundAndValid_Test()
        {
            var certificate = GetX509Certificate("thumbprint.example.com.cer");
            var result = _target.Validate(certificate);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetPrincipal_ReturnsExpectedUserFromCertificate_Test()
        {
            var expected = "Valid User";
            var certificate = GetX509Certificate("valid.example.com.cer");
            var result = _target.GetPrincipal(certificate);
            Assert.AreEqual(expected,result.Identity.Name);
        }

        [TestMethod]
        public void GetPrincipal_ReturnsExpectedUserFromThumbprint_Test()
        {
            var expected = "Thumbprint User";
            var certificate = GetX509Certificate("thumbprint.example.com.cer");
            var result = _target.GetPrincipal(certificate);
            Assert.AreEqual(expected, result.Identity.Name);
        }

        [TestMethod]
        public void GetPrincipal_ReturnsNullForUnlistedCertificate_Test()
        {
            var certificate = GetX509Certificate("unused.example.com.cer");
            var result = _target.GetPrincipal(certificate);
            Assert.IsNull(result);
        }
    }
}
