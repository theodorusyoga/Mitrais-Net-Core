using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace WebApplication1
{
    public class Startup
    {
        
        public Startup(IHostingEnvironment env)
        {
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = @"Server=MTPC433;Database=ReduxDb;Trusted_Connection=True;";
            services.AddDbContext<ReduxDbContext>(options => options.UseSqlServer(connection));

            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CORS", policy =>
            //    {
            //        policy.WithOrigins("http://localhost:8080").WithHeaders("Authorization").WithMethods("GET,PUT,POST,DELETE,OPTIONS");
            //    });
            //});

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);



            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy =>
                {
                    policy.RequireClaim("TEST", "TEST123");
                });
            });



            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors("CORS");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();



            //JWT
         
            var secret = "pass@word12345678901234567890";
            var bytes = Encoding.ASCII.GetBytes(secret);
        
            var signing = new SymmetricSecurityKey(bytes);
           
            var token = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signing,

                ValidateIssuer = true,
                ValidIssuer = "Issuer",

                ValidateAudience = true,
                ValidAudience = "Audience",

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            var options = new TokenProviderOptions
            {
                Audience = "Audience",
                Issuer = "Issuer",
                SigningCredentials = new SigningCredentials(signing, SecurityAlgorithms.HmacSha256)
            };

            app.UseMiddleware<TokenProviderMiddleware>(Options.Create(options));

            app.UseJwtBearerAuthentication(new JwtBearerOptions()
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = token
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                CookieHttpOnly= true,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                AuthenticationScheme = "Cookie",
                CookieName = "access_token",
                TicketDataFormat = new CustomJwt(
                    SecurityAlgorithms.HmacSha256,
                    token)
            });

            

            app.UseMvc();
        }
    }
}
