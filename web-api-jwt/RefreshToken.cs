using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web_api_jwt
{
    public class RefreshToken
    {

        public int Id { get; set; }
        public string Username { get; set; }
        public string Refreshtoken { get; set; }
        public bool Revoked { get; set; }
    }
}
