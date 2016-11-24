using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class TokenProviderOptions
    {
        public string Path { get; set; } = "/token";
        public string RefreshPath { get; set; } = "/refresh";
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan Expiration { get; set; } = TimeSpan.FromHours(2);
        public SigningCredentials SigningCredentials { get; set; }
    }
}
