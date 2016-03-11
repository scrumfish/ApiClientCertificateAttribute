using System.Configuration;
using System.IO;
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
        }
        private static X509Certificate2 GetX509Certificate(string fileName)
        {
            var certFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            var buffer = new byte[certFile.Length];
            certFile.Read(buffer, 0, (int)certFile.Length);
            var certificate = new X509Certificate2(buffer, "password");
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
            var certificate = GetX509Certificate("valid.example.com.pfx");
            var result = _target.Validate(certificate);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Validate_ReturnsFalseIfCertificateFoundAndExpired_Test()
        {
            var certificate = GetX509Certificate("expired.example.com.pfx");
            var result = _target.Validate(certificate);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Validate_ReturnsFalseIfCertificateNotFound_Test()
        {
            var certificate = GetX509Certificate("unused.example.com.pfx");
            var result = _target.Validate(certificate);
            Assert.IsFalse(result);
        }
    }
}
