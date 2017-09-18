using System;
using System.IO;

namespace MultithreadProducerConsumer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var tempFile = Path.GetTempFileName();

            Console.WriteLine("Results will be stored on {0} text file", tempFile);
            Console.WriteLine();
            Console.WriteLine("Press any key to leave of stop the processing...");
            Console.WriteLine();

            using (var streamWriter = new StreamWriter(tempFile))
            using (IProducerConsumer producerConsumer = new ProducerConsumer())
            {
                producerConsumer.ProducersQuantity = 5;
                producerConsumer.ProducedDataQuantityPerProducer = 10;

                producerConsumer.Writer = streamWriter;
                producerConsumer.StartConsumer();
                producerConsumer.StartProducers();

                Console.ReadKey();
            }
        }
    }
}
