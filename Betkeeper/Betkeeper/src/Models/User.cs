using System;
using System.Collections.Generic;
using System.Text;
using Betkeeper.Data;

namespace Betkeeper.Models
{
    public class User
    {
        [Column(dataType: "INTEGER", columnName: "user_id")]
        public int UserId { get; set; }

        [Column(dataType: "TEXT", columnName: "username")]
        public string Username { get; set; }

        [Column(dataType: "TEXT", columnName: "password")]
        public string Password { get; set; }
    }
}
