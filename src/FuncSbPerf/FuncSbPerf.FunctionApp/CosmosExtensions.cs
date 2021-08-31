using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FuncSbPerf.FunctionApp
{
    public static class CosmosExtensions
    {
        public static void AddCosmos(this IServiceCollection services, 
            string connectionStringName,
            bool bulkExecution)
        {
            services.AddSingleton((provider) =>
            {
                var configuration = provider.GetService<IConfiguration>();
                var connectionString = configuration[connectionStringName];

                return new CosmosClientBuilder(connectionString)
                    .WithConnectionModeDirect()
                    .WithBulkExecution(bulkExecution)
                    .Build();
            });
        }
    }
}
