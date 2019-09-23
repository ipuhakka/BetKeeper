using Api.Classes;
using Betkeeper.Exceptions;
using NUnit.Framework;

namespace Test.Api.Classes
{
    [TestFixture]
    public class TokenLogTests
    {

        [Test]
        public void GetTokenOwner_ReturnsTokenOwner()
        {
            var testToken = TokenLog.CreateToken(1);
            TokenLog.CreateToken(2);

            Assert.AreEqual(1, TokenLog.GetTokenOwner(testToken.TokenString));
        }

        [Test]
        public void GetTokenOwner_UnusedToken_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() => 
                TokenLog.GetTokenOwner("faketoken123"));
        }
    }
}
