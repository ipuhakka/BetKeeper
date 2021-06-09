using Betkeeper.Classes;
using NUnit.Framework;

namespace Betkeeper.Test.Classes
{
    [TestFixture]
    public class TokenTests
    {
        [Test]
        public void TokenConstructor_Creates24CharacterTokenString()
        {
            var token = new Token(1);

            Assert.AreEqual(24, token.TokenString.Length);
        }

        [Test]
        public void TokenConstructor_OwnerSet()
        {
            var token = new Token(1);

            Assert.AreEqual(1, token.Owner);
        }
    }
}
