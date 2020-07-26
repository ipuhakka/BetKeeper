using Betkeeper.Classes;

namespace Api.Classes
{
    public class Token
    {
        public string TokenString { get; }

        public int Owner { get; }

        public Token(int userId)
        {
            Owner = userId;
            TokenString = StringUtils.GenerateRandomString(12);
        }
    }
}