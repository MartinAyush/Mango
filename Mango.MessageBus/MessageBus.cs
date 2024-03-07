using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        private string connectionString = "Endpoint=sb://mangoserviceswebapi.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8pwSLhBiK89VhJNW1kahK1TYB98rAwZW/+ASbEJdDO8=";
        
        public async Task PublishMessage(object message, string topic_queue_Name)
        {
            await using var client = new ServiceBusClient(connectionString);
            
            ServiceBusSender sender =  client.CreateSender(topic_queue_Name);

            string jsonString = JsonConvert.SerializeObject(message);

            ServiceBusMessage busMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonString))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(busMessage);
            await client.DisposeAsync();
        }
    }
}
