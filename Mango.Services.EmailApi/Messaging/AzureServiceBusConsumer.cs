using Azure.Messaging.ServiceBus;
using Mango.Services.EmailApi.Message;
using Mango.Services.EmailApi.Models.Dto;
using Mango.Services.EmailApi.Services;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Identity.Client.Extensibility;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Mango.Services.EmailApi.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly string registerUserQueue;
        private readonly string orderCreated_Topic;
        private readonly string orderCreated_Email_Subscription;
        private ServiceBusProcessor _emailOrderPlacedProcessor;
        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registerUserProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            this._configuration = configuration;
            this._emailService = emailService;
            serviceBusConnectionString = _configuration["ServiceBusConnectionString"];
            emailCartQueue = _configuration["TopicAndQueueNames:EmailShoppingCartQueue"];
            registerUserQueue = _configuration["TopicAndQueueNames:registerUserQueue"];
            orderCreated_Topic = _configuration["TopicAndQueueNames:OrderCreateTopic"];
            orderCreated_Email_Subscription = _configuration["TopicAndQueueNames:OrderCreated_Email_Subscription"];

            var client = new ServiceBusClient(serviceBusConnectionString);

            // When we need to listen to a queue or topic we need a processor.
            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(registerUserQueue);
            _emailOrderPlacedProcessor = client.CreateProcessor(orderCreated_Topic, orderCreated_Email_Subscription);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;

            _registerUserProcessor.ProcessMessageAsync += OnRegisterUserRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;

            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;

            await _emailCartProcessor.StartProcessingAsync();
            await _registerUserProcessor.StartProcessingAsync();
            await _emailOrderPlacedProcessor.StartProcessingAsync();

        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardMessage objMessage = JsonConvert.DeserializeObject<RewardMessage>(body);

            try
            {
                //TODO - try to log mail
                await _emailService.LogOrderPlaced(objMessage);
                await arg.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task OnRegisterUserRequestReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body);

            try
            {
                //TODO - try to log mail
                await _emailService.RegisterUserAndLog(email);
                await arg.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs arg)
        {
            var message = arg.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(body);

            try
            {
                //TODO - try to log mail
                await _emailService.EmailCartAndLog(cartDto);
                await arg.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {   
                throw;
            }
        }
    }
}
