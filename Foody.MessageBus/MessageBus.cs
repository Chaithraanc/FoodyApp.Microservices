using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Foody.MessageBus
{
    public class MessageBus : IMessageBus
    {
      //  "ServiceBusConnection": "Endpoint=sb://your-azure-service-bus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=REPLACE_WITH_YOUR_SECRET"

        private string ConnectionString = "Endpoint=sb://your-azure-service-bus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=REPLACE_WITH_YOUR_SECRET";
        public async Task PublishMessage(object message, string topic_queue_Name )
        {
             await using var client = new ServiceBusClient(ConnectionString);
            ServiceBusSender sender = client.CreateSender(topic_queue_Name);

            var jsonMessage = JsonConvert.SerializeObject(message);
            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),

            };

            await sender.SendMessageAsync(finalMessage);
            await sender.DisposeAsync();
        }
    }
}
