using Foody.Services.EmailAPI.Messaging;

namespace Foody.Services.EmailAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetRequiredService<IAzureServiceBusConsumer>();
            
            var hostApplicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            hostApplicationLifetime.ApplicationStarted.Register(OnStart);
            hostApplicationLifetime.ApplicationStopping.Register(OnStop);

            return app;

        }

        private static void OnStart()
        {
            ServiceBusConsumer.Start();
        }
        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }
    }
}
