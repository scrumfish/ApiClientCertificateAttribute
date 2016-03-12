using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scrumfish.WebApi.v2.Objects.Config;

namespace Scrumfish.WebApi.v2.Objects.Tests
{
    [TestClass]
    public class SimpleProviderSection_Tests
    {
        private SimpleProviderSection _target;

        [TestInitialize]
        public void Setup()
        {
            _target = SimpleProviderSection.GetProviderSection();
        }

        [TestMethod]
        public void Target_IsNotNull_Test()
        {
            Assert.IsNotNull(_target);
        }

        [TestMethod]
        public void Users_ContainsAllExpectedUsers_Test()
        {
            var expected = new List<string> { "First User", "Second User", "Third User" };
            var users = from ConfigUserElement u in _target.Users
                        select u.UserName;
            CollectionAssert.AreEquivalent(expected, users.ToList());
        }

        [TestMethod]
        public void Users_ContainsAllExpectedCertificates_Test()
        {
            var expected = new List<string> { "a certificate base64 encoded", "some certificate text base64 encoded", string.Empty };
            var certificates = from ConfigUserElement u in _target.Users
                               select u.Certificate;
            CollectionAssert.AreEquivalent(expected, certificates.ToList());
        }

        [TestMethod]
        public void Users_ContainsAllExpectedThumbprints_Test()
        {
            var expected = new List<string> { string.Empty, string.Empty, "a thumbprint" };
            var thumbprints = from ConfigUserElement u in _target.Users
                               select u.Thumbprint;
            CollectionAssert.AreEquivalent(expected, thumbprints.ToList());
        }

        [TestMethod]
        public void Claims_ContainsAllExpectedClaims_Test()
        {
            var expected = new List<string>
            {
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                "http://schemas.scrumfish.com/ws/2016/03/rock/acdc"
            };
            var user = (from ConfigUserElement u in _target.Users
                        select u).Skip(1).FirstOrDefault();
            var claims = (from ClaimElement c in user.Claims
                          select c.Claim).ToList();
            CollectionAssert.AreEquivalent(expected, claims);
        }
        
        [TestMethod]
        public void Claims_ContainsAllExpectedValues_Test()
        {
            var expected = new List<string>
            {
                "second.user@example.com",
                "userse0039@domain.example.com",
                "User",
                "Administrator",
                "Back in Black"
            };
            var user = (from ConfigUserElement u in _target.Users
                        select u).Skip(1).FirstOrDefault();
            var values = (from ClaimElement c in user.Claims
                          select c.Value).ToList();
            CollectionAssert.AreEquivalent(expected, values);
        }

        [TestMethod]
        public void Claims_ContainsAllExpectedKeys_Test()
        {
            var expected = new List<string>
            {
                "claim1", "claim2","claim3","claim4","claim5"
            };
            var user = (from ConfigUserElement u in _target.Users
                        select u).Skip(1).FirstOrDefault();
            var keys = (from ClaimElement c in user.Claims
                          select c.Key).ToList();
            CollectionAssert.AreEquivalent(expected, keys);
        }
    }
}
