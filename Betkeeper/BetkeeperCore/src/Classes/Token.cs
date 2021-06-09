using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betkeeper.Classes
{
    public class Token
    {
        public string TokenString { get; }

        public int Owner { get; }

        /// <summary>
        /// Generates a new token
        /// </summary>
        /// <param name="userId"></param>
        public Token(int userId)
        {
            Owner = userId;
            TokenString = StringUtils.GenerateRandomString(24);
        }
    }
}
