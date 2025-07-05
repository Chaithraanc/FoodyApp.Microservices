using Azure.Messaging.ServiceBus;
using Foody.Services.RewardsAPI.Message;
using Foody.Services.RewardsAPI.Services;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.Text;

namespace Foody.Services.RewardsAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string serviceBusConnectionString;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardSubscription;
        private ServiceBusProcessor _rewardsProcessor;

        private readonly RewardService _rewardService;
        public AzureServiceBusConsumer(IConfiguration configuration , RewardService rewardService)
        {
            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
            orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
            orderCreatedRewardSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedRewardsSubscription");

            var client = new ServiceBusClient(serviceBusConnectionString);
            _rewardsProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardSubscription);

            _rewardService = rewardService;



        }
        public void Consume()
        {
            // Logic to consume messages from Azure Service Bus
            // This could involve setting up a listener, processing messages, etc.
        }

        public async Task Start()
        {
            _rewardsProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardsProcessor.ProcessErrorAsync += ErrorHandler;
            await _rewardsProcessor.StartProcessingAsync();

          
        }

      
        public async Task Stop()
        {
            await _rewardsProcessor.StopProcessingAsync();
            await _rewardsProcessor.DisposeAsync();

    
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error processing message: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
          
            try
            {
                //TODO - try to log email
                await _rewardService.UpdateRewards(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }

        }


     
    }
}

