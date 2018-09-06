using System;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace API
{
    public class Token
    {
        [JsonProperty]
        private string token;
        private int owner_id;

        public string GetToken() { return token; }
        public int GetOwnerId() { return owner_id; }

        public Token(int id)
        {
            owner_id = id;
            token = CreateToken();
            while (TokenLog.ContainsToken(token)) //create a new token until there is a unique one created.
                token = CreateToken();
        }

        private string CreateToken()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                for (int i = 0; i < 12; i++)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(valid[(int)(num % (uint)valid.Length)]);
                } 
            }
            return res.ToString();
        }
    }
}
