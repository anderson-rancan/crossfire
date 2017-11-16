using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadProducerConsumer
{
    /// <summary>
    /// Real implementation of the <seealso cref="IProducerConsumer"/>
    /// </summary>
    public sealed class ProducerConsumer : IProducerConsumer
    {
        private bool _disposed = false; // Flag to control whether the instance was disposed or not

        /// Collection that will be used to enqueue produced data and retrieve data to be consumed
        private BlockingCollection<string> _collectionToConsume = new BlockingCollection<string>();

        // Members for the Consumer task (one cancellation token and one task)
        private CancellationTokenSource _consumerTaskCancellationTokenSource;
        private Task _consumerTask;

        // Members for the Producer task (one cancellation token and one task)
        private CancellationTokenSource _producerTaskCancellationTokenSource;
        private Task _producerTask;

        // Any string to be used as a "salt" for our tiny encription example
        private const string Salt = "Anything";

        /// <summary>
        /// Default quantity of producers
        /// </summary>
        public const int DefaultProducersQuantity = 1;

        /// <summary>
        /// Default quantity of produced data per producer
        /// </summary>
        public const int DefaultProducedDataQuantityPerProducer = 1;

        #region Properties

        /// <summary>
        /// Gets or sets the <seealso cref="TextWriter"/> to write the Consumer output
        /// </summary>
        public TextWriter Writer { get; set; }

        /// <summary>
        /// Gets of sets the quantity of producers
        /// </summary>
        public int ProducersQuantity { get; set; } = DefaultProducersQuantity;

        /// <summary>
        /// Gets or sets the quantity of produced data per producer
        /// </summary>
        public int ProducedDataQuantityPerProducer { get; set; } = DefaultProducedDataQuantityPerProducer;

        #endregion

        #region Consumer logic

        /// <summary>
        /// Starts the Consumer task
        /// </summary>
        public void StartConsumer()
        {
            ThrowIfDisposed();
            ThrowIfNoWriter();
            
            if (_consumerTask != null) throw new ApplicationException("Consumer was already started");

            _consumerTaskCancellationTokenSource = new CancellationTokenSource();

            // collect the data to pass to the new task on declaration time
            var consumerData = new ConsumerData
            {
                Action = Consume,
                Collection = _collectionToConsume,
                Token = _consumerTaskCancellationTokenSource.Token
            };

            _consumerTask = Task.Factory.StartNew(
                Consumer,                                       // method that represents the new task
                consumerData,                                   // passed to the method as an parameter of type object
                _consumerTaskCancellationTokenSource.Token,     // our new cancellation token, so we can cancel the task
                TaskCreationOptions.LongRunning,                // we expect that this will be a long running task in a real implementation
                TaskScheduler.Default);                         // we will not manage the Scheduler, let`s just pass the default one
        }

        /// <summary>
        /// Method that will be executed by the Consumer task
        /// </summary>
        /// <param name="data">Task data of type <seealso cref="ConsumerData"/></param>
        private void Consumer(object data)
        {
            var consumerData = data as ConsumerData;
            if (consumerData == null) throw new ArgumentException($"Argument type is not {nameof(ConsumerData)}", nameof(data));

            Console.WriteLine("Consumer task started");

            try
            {
                // using the GetConsumingEnumerable we are using the producer-consumer implementation
                // it will remove the item from the collection to be processed
                // if there are no new items to be removed, this method will block the current thread and wait for new data
                // it will unblock or throw exceptions the the cancellation token is used, disposed, or if the collection is flagged as not accepting more additions
                foreach (var item in consumerData.Collection.GetConsumingEnumerable(consumerData.Token))
                {
                    consumerData.Action(item); // calls the action to consume data
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Consumer task was canceled");
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Consumer task or its cancellation token has been disposed");
            }

            Console.WriteLine("Consumer task finished");
        }

        /// <summary>
        /// Consumes the provided data
        /// </summary>
        /// <param name="data">Data as a <c>string</c> to be consumed</param>
        /// <remarks>
        /// This is just a tiny example of a data consumer.
        /// This method will receive a protected string, unprotect it and add it to the provided writer
        /// </remarks>
        private void Consume(object data)
        {
            var receivedData = data as string;
            if (receivedData == null) throw new ArgumentException("Argument type is not string", nameof(data));

            var protectedText = ProtectedData.Unprotect(
                Convert.FromBase64String(receivedData),
                Encoding.Default.GetBytes(Salt),
                DataProtectionScope.CurrentUser);

            var text = Encoding.Default.GetString(protectedText);

            Writer.WriteLine(text);
        }

        #endregion

        #region Producer logic

        /// <summary>
        /// Starts the Producer task
        /// </summary>
        public void StartProducers()
        {
            ThrowIfDisposed();
            ThrowIfNoWriter();

            Console.WriteLine("Starting producers");

            _producerTaskCancellationTokenSource = new CancellationTokenSource();

            // starts a new task with an action and an additional cancellation token as an argument
            _producerTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    // here I'm using a parallel for just to demonstrate several producers enqueuing new data on the collection
                    Parallel.For(1, ProducersQuantity + 1, (counter) =>
                    {
                        Producer(Produce(counter)); // call the method to produced and enqueue new data
                        Console.WriteLine("Producer {0} finished", counter);
                    });

                    // flags the collection that it won't receive new data
                    // after flagged, when it runs empty of data it won't block the thread anymore, it will just exit
                    _collectionToConsume.CompleteAdding(); 

                    Console.WriteLine();
                    Console.WriteLine("Every producer finished adding new data");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Producer task was canceled");
                }
            },
            _producerTaskCancellationTokenSource.Token);
        }

        /// <summary>
        /// Enqueue the produced data on the collection
        /// </summary>
        /// <param name="producedDataList">Produced data</param>
        private void Producer(List<string> producedDataList)
        {
            // simulates a delay, as in a real implementation network/cpu/amount of data may need a different amount of time
            Thread.Sleep(200);

            // enqueue data on the collection
            producedDataList.ForEach(_collectionToConsume.Add);
        }

        /// <summary>
        /// Produces data
        /// </summary>
        /// <param name="producer">Quantity of data to be produced</param>
        /// <returns>List of produced data</returns>
        /// <remarks>
        /// This method simulates a producer.
        /// It will protect a string and create a list with the specified number of protected strings
        /// </remarks>
        private List<string> Produce(int producer)
        {
            var result = new List<string>();

            for (int counter = 0; counter < ProducedDataQuantityPerProducer; counter++)
            {
                var text = $"Hello world (counter: {counter}) (producer: {producer})";

                var protectedText = ProtectedData.Protect(
                    Encoding.Default.GetBytes(text),
                    Encoding.Default.GetBytes(Salt),
                    DataProtectionScope.CurrentUser);

                result.Add(Convert.ToBase64String(protectedText));
            }

            return result;
        }

        #endregion

        #region Assert

        /// <summary>
        /// Throws an exception if no writer was specified
        /// </summary>
        private void ThrowIfNoWriter()
        {
            if (Writer == null) throw new NullReferenceException($"{Writer} should not be null");
        }

        /// <summary>
        /// Throws an exception if the instance was disposed
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ProducerConsumer));
        }

        #endregion

        #region Cleaning logic

        /// <summary>
        /// Stops and disposes the Producer task and token
        /// </summary>
        private void StopAndDiposeProducer()
        {
            if (_producerTaskCancellationTokenSource != null && !_producerTaskCancellationTokenSource.IsCancellationRequested)
            {
                _producerTaskCancellationTokenSource.Cancel();
            }

            if (_producerTask != null)
            {
                _producerTask.Wait(500);
                _producerTask.Dispose();
                _producerTask = null;
            }

            if (_producerTaskCancellationTokenSource != null)
            {
                _producerTaskCancellationTokenSource.Dispose();
                _producerTaskCancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Stops and disposes the Consumer task and token
        /// </summary>
        public void StopAndDisposeConsumer()
        {
            if (_consumerTaskCancellationTokenSource != null && !_consumerTaskCancellationTokenSource.IsCancellationRequested)
            {
                _consumerTaskCancellationTokenSource.Cancel();
            }

            if (_consumerTask != null)
            {
                _consumerTask.Wait(500);
                _consumerTask.Dispose();
                _consumerTask = null;
            }

            if (_consumerTaskCancellationTokenSource != null)
            {
                _consumerTaskCancellationTokenSource.Dispose();
                _consumerTaskCancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            StopAndDiposeProducer();

            StopAndDisposeConsumer();

            if (_collectionToConsume != null)
            {
                _collectionToConsume.Dispose();
                _collectionToConsume = null;
            }

            _disposed = true;
        }

        #endregion

    }
}
