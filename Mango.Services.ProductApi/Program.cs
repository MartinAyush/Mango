
using AutoMapper;
using Mango.Services.ProductApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mango.Services.ProductApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddDbContext<AppDbContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
			});

			// Auto mapper configuration
			IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
			builder.Services.AddSingleton(mapper);
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();


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

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AUTH API");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();
			CheckAndApplyMigrations(app);
			app.Run();
		}

		public static void CheckAndApplyMigrations(WebApplication app)
		{
			using (var scope = app.Services.CreateScope())
			{
				var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

				if (db.Database.GetPendingMigrations().Count() > 0)
				{
					db.Database.Migrate();
				}
			}
		}
	}
}
