using System.Collections.Generic;
using System.Threading.Tasks;
using FuncSbPerf.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FuncSbPerf.FunctionApp
{
    public class SendMessage
    {
        private CosmosClient _cosmosClient;
        private ILogger _log;
        private Container _container;

        public SendMessage(
            CosmosClient cosmosClient, 
            ILogger<SendMessage> log)
        {
            _cosmosClient = cosmosClient;
            _log = log;

            _container = _cosmosClient.GetContainer(Globals.Cosmos.DatabaseName, Globals.Cosmos.CollectionName);
        }

        [FunctionName("SendMessage")]
        public async Task Run(
             [ServiceBusTrigger(
                Globals.ServiceBus.ProcessedQueueName, 
                Connection = Globals.ServiceBus.ConnectionStringName,
                AutoComplete = false)] Message[] messages, MessageReceiver messageReceiver)
        {
            _log.LogInformation($"SendMessage Called with {messages.Length} messages.");

            // Create a c# local function or lambda to wrap with a Func<T, Task> delegate
            Task processor(Message m) => ProcessAsync(m, messageReceiver);

            //  Process 5 messages at a time (see TaskExtensions)
            await messages.ForEachAsync(processor, 10);
        }

        /// <summary>
        /// Writes a single message to the blob container using the message id as the file name
        /// </summary>
        /// <param name="message">an individual message to process</param>
        /// <param name="messageReceiver">the receiver object to manually complete this message</param>
        /// <returns>a task representing this operation</returns>
        private async Task ProcessAsync(Message message, MessageReceiver messageReceiver)
        {
            var model = JsonUtil.Deserialize<MessageModel>(
                message.Body);

            await ProcessAsync(model);
            _log.LogInformation($"C# ServiceBus queue trigger function processed message: {message.MessageId}");

            await messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ProcessAsync(MessageModel model)
        {
            return _container.CreateItemAsync(
                model, 
                new PartitionKey(model.Id.ToString()));
        }
    }
}

