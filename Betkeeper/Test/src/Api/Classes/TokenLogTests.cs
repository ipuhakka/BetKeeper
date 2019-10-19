using System.Net.Http;
using Api.Classes;
using Betkeeper.Exceptions;
using NUnit.Framework;

namespace Test.Api.Classes
{
    [TestFixture]
    public class TokenLogTests
    {

        [TearDown]
        public void TearDown()
        {
            TokenLog.ClearTokenLog();
        }

        [Test]
        public void GetExistingToken_UserHasToken_ReturnsToken()
        {
            var testToken = TokenLog.CreateToken(1);

            Assert.AreEqual(testToken.TokenString, 
                TokenLog.GetExistingToken(1).TokenString);
            Assert.AreEqual(testToken.Owner, 1);
        }

        [Test]
        public void GetExistingToken_UserDoesNotHaveToken_ReturnsNull()
        {
            TokenLog.CreateToken(1);

            Assert.IsNull(TokenLog.GetExistingToken(2));
        }

        [Test]
        public void GetTokenOwner_ReturnsTokenOwner()
        {
            var testToken = TokenLog.CreateToken(1);
            TokenLog.CreateToken(2);

            Assert.AreEqual(1, TokenLog.GetTokenOwner(testToken.TokenString));
        }

        [Test]
        public void GetTokenOwner_UnusedToken_ReturnsNull()
        {
            Assert.IsNull(TokenLog.GetTokenOwner("faketoken123"));
        }

        [Test]
        public void DeleteToken_DoesNotBelongToUser_ThrowsNotFoundException()
        {
            TokenLog.CreateToken(1);
            var token2 = TokenLog.CreateToken(2);

            Assert.Throws<NotFoundException>(() =>
                TokenLog.DeleteToken(1, token2.TokenString));

            Assert.IsTrue(TokenLog.ContainsToken(token2.TokenString));
        }

        [Test]
        public void DeleteToken_DoesBelongToUser_TokenDeleted()
        {
            TokenLog.CreateToken(1);
            var token2 = TokenLog.CreateToken(2);

            TokenLog.DeleteToken(2, token2.TokenString);

            Assert.IsFalse(TokenLog.ContainsToken(token2.TokenString));
        }

        [Test]
        public void GetUserIdFromRequest_UserDoesNotHaveToken_ReturnsNull()
        {
            var request = new HttpRequestMessage();

            request.Headers.Add("Authorization", "tokenDoesNotExist");

            Assert.IsNull(TokenLog.GetUserIdFromRequest(request));
        }

        [Test]
        public void GetUserIdFromRequest_UserHasToken_ReturnsUserId()
        {
            TokenLog.CreateToken(1);
            var testToken = TokenLog.CreateToken(2);

            var request = new HttpRequestMessage();

            request.Headers.Add("Authorization", testToken.TokenString);

            Assert.AreEqual(2, TokenLog.GetUserIdFromRequest(request));
        }
    }
}
