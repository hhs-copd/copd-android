using System;

namespace LocationTest.Services
{
    public class LoginResult
    {
        public string Username { get; set; }

        public string RefreshToken { get; set; }

        public string AccessToken { get; set; }

        public DateTime AccessTokenExpiration { get; set; }

        public bool Error { get; set; }

        public string IdToken { get; set; }
    }
}
