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

namespace WebApplication1
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        public TokenProviderMiddleware(
            RequestDelegate next, IOptions<TokenProviderOptions> options)
        {
            this._next = next;
            this._options = options.Value;
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

            var identity = await GetIdentity(username, password);
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
            if (username == "TEST" && password == "TEST123")
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(username, "Token")
                    , new Claim[] {
                           new Claim(username, password)
                    }));

            return Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
