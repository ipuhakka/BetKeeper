using System;
using System.Security.Cryptography;
using System.Text;

namespace Api.Classes
{
    public class Token
    {
        public string TokenString { get; }

        public int Owner { get; }

        public Token(int userId)
        {
            Owner = userId;
            TokenString = GenerateToken();
        }

        /// <summary>
        /// Creates a 12-character token string and returns it.
        /// </summary>
        /// <returns></returns>
        private string GenerateToken()
        {
            const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            StringBuilder token = new StringBuilder();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                for (int i = 0; i < 12; i++)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    token.Append(validCharacters[(int)(num % (uint)validCharacters.Length)]);
                }
            }

            return token.ToString();
        }
    }
}