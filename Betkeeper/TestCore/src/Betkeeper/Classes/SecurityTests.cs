using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Classes;
using NUnit.Framework;

namespace Betkeeper.Test.Classes
{
    [TestFixture]
    public class SecurityTests
    {
        [Test]
        public void EncryptDecrypt_ReturnsExpectedOutput()
        {
            Settings.SecretKey = "b14ca5898a4e4133bbce2ea2315a1916";

            Assert.AreEqual(Security.Decrypt(Security.Encrypt("test")), "test");

            Assert.AreEqual(Security.Decrypt(Security.Encrypt("TEST")), "TEST");

            Assert.AreEqual(Security.Decrypt(Security.Encrypt("123!_&hH%")), "123!_&hH%");
        }
    }
}
