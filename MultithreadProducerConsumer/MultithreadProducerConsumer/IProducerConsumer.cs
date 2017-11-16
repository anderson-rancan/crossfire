using System;
using System.IO;

namespace MultithreadProducerConsumer
{
    /// <summary>
    /// Interface for the producer-consumer example
    /// </summary>
    public interface IProducerConsumer : IDisposable
    {
        /// <summary>
        /// Gets of sets the quantity of producers
        /// </summary>
        int ProducersQuantity { get; set; }

        /// <summary>
        /// Gets or sets the quantity of produced data per producer
        /// </summary>
        int ProducedDataQuantityPerProducer { get; set; }

        /// <summary>
        /// Starts the Consumer task
        /// </summary>
        void StartConsumer();

        /// <summary>
        /// Starts the Producer task
        /// </summary>
        void StartProducers();

        /// <summary>
        /// Gets or sets the <seealso cref="TextWriter"/> to write the Consumer output
        /// </summary>
        TextWriter Writer { get; set; }
    }
}
