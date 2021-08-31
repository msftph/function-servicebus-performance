using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FuncSbPerf.FunctionApp
{
    public class ProcessMessages
    {
        private ILogger _log;

        public ProcessMessages(ILogger<ProcessMessages> log)
        {
            _log = log;
        }

        [FunctionName("ProcessMessages")]
        public async Task RunAsync(
            [ServiceBusTrigger(
                Globals.ServiceBus.ReceivedQueueName, 
                Connection = Globals.ServiceBus.ConnectionStringName,
                AutoComplete = false)] Message[] messages, MessageReceiver messageReceiver,
            [ServiceBus(Globals.ServiceBus.ProcessedQueueName, Connection = Globals.ServiceBus.ConnectionStringName)]IAsyncCollector<Message> collector)
        {
            _log.LogInformation($"ProcessMessages Called with {messages.Length} messages.");

            // Create a c# local function or lambda to wrap with a Func<T, Task> delegate
            Task processor(Message m) => ProcessAsync(m, messageReceiver, collector);

            //  Process 5 messages at a time (see TaskExtensions)
            await messages.ForEachAsync(processor, 5);
        }

        /// <summary>
        /// Writes a single message to the blob container using the message id as the file name
        /// </summary>
        /// <param name="message">an individual message to process</param>
        /// <param name="messageReceiver">the receiver object to manually complete this message</param>
        /// <param name="container">the container to write the message</param>
        /// <param name="log">the log for processing the message</param>
        /// <returns>a task representing this operation</returns>
        private async Task ProcessAsync(Message message, MessageReceiver messageReceiver, IAsyncCollector<Message> collector)
        {
            // make sure to clone the message so we don't get the "message already received" exception
            await collector.AddAsync(message.Clone());
            _log.LogInformation($"C# ServiceBus queue trigger function processed message: {message.MessageId}");
            await messageReceiver.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
