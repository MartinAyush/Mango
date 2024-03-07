
using Mango.Services.EmailApi.Data;
using Mango.Services.EmailApi.Extensions;
using Mango.Services.EmailApi.Messaging;
using Mango.Services.EmailApi.Services;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailApi
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

            // we cannot use a scoped services inside a singleton service
            // singleton service - created/initialized for litetime of application
            // scoped service - created per user request
            var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            builder.Services.AddSingleton(new EmailService(optionBuilder.Options));

            builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AUTH API");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            CheckAndApplyMigrations(app);
            app.UseAzureServiceBusConsumer();
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
