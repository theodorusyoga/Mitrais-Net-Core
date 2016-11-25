using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Security.Principal;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        private ReduxDbContext _context;
        public TokenProviderMiddleware(
            RequestDelegate next, IOptions<TokenProviderOptions> options, ReduxDbContext context)
        {
            this._next = next;
            this._options = options.Value;
            this._context = context;
        }
        public Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
                return _next(context);


            if (!context.Request.Method.Equals("POST") ||
                !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                //  context.Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));
                return context.Response.WriteAsync("Bad request");
            }
            
                return GenerateToken(context);
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            var sha = password.ToString().Sha256Encrypt();

            var identity = await GetIdentity(username, sha);
            if (identity == null)
            {
                context.Response.StatusCode = 400;
                //  context.Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));
                await context.Response.WriteAsync("Invalid username or password");
                return;
            }

            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.NameId, identity.FindFirst("ID").Value),
                identity.FindFirst("TEST")
            };
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };

            context.Response.ContentType = "application/json";
            //  context.Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings() { Formatting = Formatting.Indented }));
        }

      

        private Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            var select = this._context.Users.Where(p => p.Username.Equals(username) && p.Password.Equals(password));
            if (select.Count() == 1)
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity("TEST", "Token")
                    , new Claim[] {
                           new Claim("TEST", "TEST123"),
                           new Claim("ID", select.Single().ID.ToString())
                    }));

            return Task.FromResult<ClaimsIdentity>(null);
        }


    }
}
