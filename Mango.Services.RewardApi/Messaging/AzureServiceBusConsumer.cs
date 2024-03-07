using Azure.Messaging.ServiceBus;
using Mango.Services.RewardApi.Message;
using Mango.Services.RewardApi.Messaging;
using Mango.Services.RewardApi.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly RewardService _rewardService;
        private readonly string serviceBusConnectionString;
        private readonly string OrderCreateTopic;
        private readonly string OrderCreated_Rewards_Subscription;
        private ServiceBusProcessor _rewardProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            this._configuration = configuration;
            this._rewardService = rewardService;

            serviceBusConnectionString = _configuration["ServiceBusConnectionString"];
            OrderCreateTopic = _configuration["TopicAndQueueNames:OrderCreateTopic"];
            OrderCreated_Rewards_Subscription = _configuration["TopicAndQueueNames:OrderCreated_Rewards_Subscription"];

            var client = new ServiceBusClient(serviceBusConnectionString);

            // When we need to listen to a queue or topic we need a processor.
            _rewardProcessor = client.CreateProcessor(OrderCreateTopic, OrderCreated_Rewards_Subscription);
        }

        public async Task Start()
        {
            _rewardProcessor.ProcessMessageAsync += OnNewOrderRewardRequestReceived;
            _rewardProcessor.ProcessErrorAsync += ErrorHandler;
            
            await _rewardProcessor.StartProcessingAsync();
        }

        private async Task OnNewOrderRewardRequestReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            var objMessage = JsonConvert.DeserializeObject<RewardMessage>(body);

            try
            {
                //TODO - try to log mail
                await _rewardService.UpdateRewards(objMessage);
                await arg.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Stop()
        {
            await _rewardProcessor.StopProcessingAsync();
            await _rewardProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
