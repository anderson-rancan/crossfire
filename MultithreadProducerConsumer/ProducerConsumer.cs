using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadProducerConsumer
{
    public interface IProducerConsumer : IDisposable
    {
        void StartConsumer();

        void StopConsumer();

        Task StartProducers(int numberOfProducers, object data);
    }

    public sealed class ProducerConsumer : IProducerConsumer
    {
        private BlockingCollection<string> _collectionToConsume = new BlockingCollection<string>();

        private CancellationTokenSource _consumerTaskCancellationTokenSource;
        private Task _consumerTask;

        private List<Task> _producerTasks = new List<Task>();

        public void StartConsumer()
        {
            if (_consumerTask != null) throw new ApplicationException("Consumer was already started");

            _consumerTaskCancellationTokenSource = new CancellationTokenSource();

            var consumerData = new ConsumerData
            {
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

        public void StopConsumer()
        {
            if (_consumerTaskCancellationTokenSource != null && !_consumerTaskCancellationTokenSource.IsCancellationRequested)
            {
                _consumerTaskCancellationTokenSource.Cancel();
            }

            _consumerTask.Wait(500);

            DisposeConsumer();
        }

        public async Task StartProducers(int numberOfProducers, object data)
        {
            await Task.Delay(2000);
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
                    throw new NotImplementedException();
                }
            }
            catch (OperationCanceledException) { /* Task was canceled */ }

            Console.WriteLine("Consumer task finished");
        }

        private void DisposeConsumer()
        {
            _consumerTask.Dispose();
            _consumerTask = null;

            _consumerTaskCancellationTokenSource.Dispose();
            _consumerTaskCancellationTokenSource = null;
        }

        public void Dispose()
        {
            StopConsumer();
        }
    }

    public class ConsumerData
    {
        public BlockingCollection<string> Collection { get; set; }

        public CancellationToken Token { get; set; }
    }
}
