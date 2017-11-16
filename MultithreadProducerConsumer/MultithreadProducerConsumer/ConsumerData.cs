using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MultithreadProducerConsumer
{
    /// <summary>
    /// Consumer data to be used by the Consumer task
    /// </summary>
    public class ConsumerData
    {
        /// <summary>
        /// Action to be executed as a simulated consumer
        /// </summary>
        public Action<string> Action { get; set; }

        /// <summary>
        /// Reference to the collection that will receive produced data
        /// </summary>
        public BlockingCollection<string> Collection { get; set; }

        /// <summary>
        /// Token to trigger the cancellation of the Consumer task
        /// </summary>
        public CancellationToken Token { get; set; }
    }
}
