using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using System.Collections.Generic;
using FuncSbPerf.Shared;

namespace FuncSbPerf.FunctionApp
{
    public class ReceiveMessage
    {
        private readonly ILogger _log;

        public ReceiveMessage(ILogger<ReceiveMessage> log)
        {
            _log = log;
        }

        [FunctionName("ReceiveMessage")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [ServiceBus("MyReceivedMessages", Connection = "MyMessages")] IAsyncCollector<Message> collector)
        {
            _log.LogInformation("Message Received");

            var request = JsonUtil.Deserialize<RequestModel>(req.Body);
            var tasks = new List<Task>();
            foreach (var message in request.Messages)
            {
                var bytes = JsonUtil.SerializeToByteArray(message);
                var serviceBusMessage = new Message(bytes);                                
                var task = collector.AddAsync(serviceBusMessage);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
            return new OkResult();
        }
    }
}
