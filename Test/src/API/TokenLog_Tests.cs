using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API;
using NUnit.Framework;

namespace Test.API
{
    [TestFixture]
    [Category("API")]
    class TokenLog_Tests
    {
        [Test]
        public void test_ContainsToken_return_true()
        {
            Token t = new Token(1);
            TokenLog.AddToken(t);
            Assert.IsTrue(TokenLog.ContainsToken(t.GetToken()));
        }

        [Test]
        public void test_ContainsToken_return_false()
        {
            Token t = new Token(2);
            TokenLog.AddToken(t);

            Token t2 = new Token(3);
            Assert.IsFalse(TokenLog.ContainsToken(t2.GetToken()));
        }

        [Test]
        public void test_GetTokenOwner_return4()
        {
            Token t = new Token(4);
            TokenLog.AddToken(t);
            Assert.AreEqual(4, TokenLog.GetTokenOwner(t.GetToken()));
        }

        [Test]
        public void test_GetTokenOwner_returnminus1()
        {
            Token t = new Token(5);
            TokenLog.AddToken(t);
            Token t2 = new Token(6);
            Assert.AreEqual(-1, TokenLog.GetTokenOwner(t2.GetToken()));
        }


    }
}
