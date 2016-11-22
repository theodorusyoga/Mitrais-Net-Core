using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1
{
    public class CustomJwt : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly string algorithm;
        private readonly TokenValidationParameters token;
        public CustomJwt(string algorithm, TokenValidationParameters token)
        {
            this.algorithm = algorithm;
            this.token = token;
        }
        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException();
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket Unprotect(string protectedText)
        => Unprotect(protectedText, null);

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal;
            SecurityToken validtoken;

            try
            {
                principal = handler.ValidateToken(protectedText, this.token, out validtoken);
                
                var validjwt = validtoken as JwtSecurityToken;
                if(validjwt == null)
                {
                    throw new ArgumentException("Invalid JWT");
                }
                
                if(!validjwt.Header.Alg.Equals(algorithm, StringComparison.Ordinal))
                {
                    throw new ArgumentException("Algorithm must be " + this.algorithm);
                }
            }
            catch(SecurityTokenValidationException e)
            {
                return null;
            }
            catch(ArgumentException e)
            {
                return null;
            }

            return new AuthenticationTicket(principal, new AuthenticationProperties(), "Cookie");
        }
    }
}
