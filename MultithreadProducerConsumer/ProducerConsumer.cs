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
    public interface IProducerConsumer : IDisposable
    {
        int ProducersQuantity { get; set; }

        int ProducedDataQuantityPerProducer { get; set; }

        void StartConsumer();

        void StartProducers();

        TextWriter Writer { get; set; }
    }

    public sealed class ProducerConsumer : IProducerConsumer
    {
        private bool _disposed = false;

        private BlockingCollection<string> _collectionToConsume = new BlockingCollection<string>();

        private CancellationTokenSource _consumerTaskCancellationTokenSource;
        private Task _consumerTask;

        private CancellationTokenSource _producerTaskCancellationTokenSource;
        private Task _producerTask;

        private const string Salt = "Anything";

        #region Properties

        public TextWriter Writer { get; set; }

        public int ProducersQuantity { get; set; } = 1;

        public int ProducedDataQuantityPerProducer { get; set; } = 1;

        #endregion

        #region Consumer logic

        public void StartConsumer()
        {
            ThrowIfDisposed();
            ThrowIfNotWriter();
            
            if (_consumerTask != null) throw new ApplicationException("Consumer was already started");

            _consumerTaskCancellationTokenSource = new CancellationTokenSource();

            var consumerData = new ConsumerData
            {
                Action = Consume,
                Collection = _collectionToConsume,
                Token = _consumerTaskCancellationTokenSource.Token
            };

            _consumerTask = Task.Factory.StartNew(
                Consumer,
                consumerData,
                _consumerTaskCancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);
        }

        private void Consumer(object data)
        {
            var consumerData = data as ConsumerData;
            if (consumerData == null) throw new ArgumentException($"Argument type is not {nameof(ConsumerData)}", nameof(data));

            Console.WriteLine("Consumer task started");

            try
            {
                foreach (var item in consumerData.Collection.GetConsumingEnumerable(consumerData.Token))
                {
                    consumerData.Action(item);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Consumer task was canceled");
            }

            Console.WriteLine("Consumer task finished");
        }

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

        public void StartProducers()
        {
            ThrowIfDisposed();
            ThrowIfNotWriter();

            Console.WriteLine("Starting producers");

            _producerTaskCancellationTokenSource = new CancellationTokenSource();

            _producerTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    Parallel.For(1, ProducersQuantity + 1, (counter) =>
                    {
                        Producer(Produce(counter));
                        Console.WriteLine("Producer {0} finished", counter);
                    });

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

        private void Producer(List<string> producedDataList)
        {
            Thread.Sleep(200);

            producedDataList.ForEach(_collectionToConsume.Add);
        }

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

        private void ThrowIfNotWriter()
        {
            if (Writer == null) throw new NullReferenceException($"{Writer} should not be null");
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ProducerConsumer));
        }

        #endregion

        #region Cleaning logic

        private void StopAndDiposeProducer()
        {
            if (_producerTaskCancellationTokenSource != null && !_producerTaskCancellationTokenSource.IsCancellationRequested)
            {
                _producerTaskCancellationTokenSource.Cancel();
            }

            _producerTask.Wait(500);
            _producerTask.Dispose();
            _producerTask = null;

            _producerTaskCancellationTokenSource.Dispose();
            _producerTaskCancellationTokenSource = null;
        }

        public void StopAndDisposeConsumer()
        {
            if (_consumerTaskCancellationTokenSource != null && !_consumerTaskCancellationTokenSource.IsCancellationRequested)
            {
                _consumerTaskCancellationTokenSource.Cancel();
            }

            _consumerTask.Wait(500);
            _consumerTask.Dispose();
            _consumerTask = null;

            _consumerTaskCancellationTokenSource.Dispose();
            _consumerTaskCancellationTokenSource = null;
        }

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

    public class ConsumerData
    {
        public Action<string> Action { get; set; }

        public BlockingCollection<string> Collection { get; set; }

        public CancellationToken Token { get; set; }
    }
}
