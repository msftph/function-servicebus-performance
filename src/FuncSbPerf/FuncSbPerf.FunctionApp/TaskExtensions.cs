using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FuncSbPerf.FunctionApp
{
    /// <summary>
    /// TaskExtensions provide extension methods for running tasks
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// <see cref="https://devblogs.microsoft.com/pfxteam/implementing-a-simple-foreachasync-part-2/"/>
        /// ForEachAsync creates <paramref name="maxDegreesOfConcurrency"/> partitions and enumerates each partition concurrently until all items are completed. 
        /// </summary>
        /// <typeparam name="T">The type of message to process</typeparam>
        /// <param name="items">The list of messages</param>
        /// <param name="processor">The processor function</param>
        /// <param name="maxDegreesOfConcurrency">Maximum number of concurrent workers. Value must be greater than zero</param>
        /// <returns></returns>
        public static Task ForEachAsync<T>(
            this IEnumerable<T> items, 
            Func<T, Task> processor, 
            int maxDegreesOfConcurrency)
        {
            if (maxDegreesOfConcurrency <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxDegreesOfConcurrency), $"value must be greater than zero.");

            if (processor is null)
                throw new ArgumentNullException(nameof(processor));

            if (items is null)
                throw new ArgumentNullException(nameof(items));

            var partitions = Partitioner
                .Create(items)
                .GetPartitions(maxDegreesOfConcurrency);

            var tasks = new List<Task>();
            foreach (var partition in partitions)
            {
                var task = ProcessPartition(partition, processor);
                tasks.Add(task);
            }

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// ProcessPartition awaits each item in the enumerator until all items are processed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="partition"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        private static async Task ProcessPartition<T>(IEnumerator<T> partition, Func<T, Task> processor)
        {
            // TODO: Check if this is the proper way to handle aggregate exceptions
            var exceptions = new List<Exception>();            

            while (partition.MoveNext())
                try
                {
                    await processor(partition.Current);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }
    }
}
