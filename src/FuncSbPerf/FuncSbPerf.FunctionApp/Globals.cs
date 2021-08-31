using System;
using System.Collections.Generic;
using System.Text;

namespace FuncSbPerf.FunctionApp
{
    class Globals
    {
        public class ServiceBus
        {
            public const string ConnectionStringName = "MyMessages";
            public const string ProcessedQueueName = "MyProcessedMessages";
            public const string ReceivedQueueName = "MyReceivedMessages";
        }

        public class Storage
        {
            public const string ConnectionStringName = "MySendMessages";
            public const string SentBlobName = "myblobs";
        }

        public class Cosmos
        {
            public const string CollectionName = "myMessages";
            public const string DatabaseName = "myDatabase";
            public const string ConnectionStringName = "CosmosDBConnection";
        }
    }
}
