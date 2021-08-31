using FuncSbPerf.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FuncSbPerf.ClientApp
{
    class MyApplication
    {
        readonly IHttpClientFactory _factory;
        readonly ILogger _logger;
        readonly IConfiguration _configuration;

        public MyApplication(IHttpClientFactory factory, IConfiguration configuration, ILogger<MyApplication> logger)
        {
            _factory = factory;
            _logger = logger;
            _configuration = configuration;
        }

        public Task Run(int messageCount, int windowSize)
        {
            var functionEndpointUri = _configuration[Globals.FunctionUrlName];
            return Run(functionEndpointUri, messageCount, windowSize);
        }

        private async Task Run(string functionEndpointUri, int messageCount, int windowSize)
        {
            var requestModel = new Shared.RequestModel();
            var tasks = new List<Task>();
            var client = _factory.CreateClient();
            for (var i = 0; i < messageCount; i++)
            {                
                requestModel.Messages.Add(
                    new MessageModel 
                    { 
                        Id = Guid.NewGuid(),
                        Content = $"Message {i}" 
                    });
                if (requestModel.Messages.Count < windowSize)
                    continue;

                var response = SendAsync(functionEndpointUri, requestModel, client);                
                tasks.Add(response);
                _logger.LogInformation("Message Batch Sent");

                // create a new request model for processing
                requestModel = new Shared.RequestModel();                                
            }

            if (requestModel.Messages.Count > 0)
            {
                var response = SendAsync(functionEndpointUri, requestModel, client);                
                tasks.Add(response);
                _logger.LogInformation("Message Batch Sent");
            }

            await Task.WhenAll(tasks);
        }

        private static Task<HttpResponseMessage> SendAsync(
            string functionEndpointUri, 
            RequestModel requestModel, 
            HttpClient client)
        {
            var json = JsonUtil.SerializeToString(requestModel);
            var request = new HttpRequestMessage(HttpMethod.Post, functionEndpointUri);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = content;
            var response = client.SendAsync(request);
            return response;
        }
    }
}
