using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;
using System.Text;

namespace Mango.GatewaySolution
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if(builder.Environment.EnvironmentName.ToString().ToLower().Equals("production"))
                builder.Configuration.AddJsonFile("Ocelot.Production.json", optional: false, reloadOnChange: true);
            else
                builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);

            builder.Services.AddOcelot(builder.Configuration);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"]
                };
            });
            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOcelot().GetAwaiter().GetResult();
            app.Run();
        }
    }
}
