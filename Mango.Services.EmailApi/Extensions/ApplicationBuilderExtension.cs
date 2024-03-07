using Mango.Services.EmailApi.Messaging;
using System.Reflection.Metadata;

namespace Mango.Services.EmailApi.Extensions
{
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer _serviceBusConsumer;
        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            _serviceBusConsumer = app.ApplicationServices.GetRequiredService<IAzureServiceBusConsumer>();
            var hostApplicationLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLifetime.ApplicationStarted.Register(OnStart);
            hostApplicationLifetime.ApplicationStopped.Register(OnStop);

            return app;
        }

        private static void OnStop()
        {
            _serviceBusConsumer.Stop();
        }

        private static void OnStart()
        {
            _serviceBusConsumer.Start();
        }
    }
}
