using System;
using System.Security.Cryptography;
using System.Text;

namespace Betkeeper.Classes
{
    public class StringUtils
    {
        /// <summary>
        /// Generates a random string of specified length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomString(int length)
        {
            const string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            StringBuilder randomString = new StringBuilder();

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                for (int i = 0; i < length; i++)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    randomString.Append(validCharacters[(int)(num % (uint)validCharacters.Length)]);
                }
            }

            return randomString.ToString();
        }
    }
}
