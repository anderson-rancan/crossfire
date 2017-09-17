using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultithreadProducerConsumer
{
    public static class Program
    {
        private const int ExitInput = 0;
        private const int InvalidNumberOfProducers = -1;

        public static void Main(string[] args)
        {
            var numberOfProducers = ProcessArguments(args);

            using (IProducerConsumer producerConsumer = new ProducerConsumer())
            {
                producerConsumer.StartConsumer();

                producerConsumer.StartProducers(5, string.Empty).Wait();

                Console.WriteLine("Press any key to leave");
                Console.ReadKey();
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Program was terminated, press any key to leave");
            Console.ReadKey();
        }

        public static int ProcessArguments(string[] args)
        {
            int numberOfProducers = 0;

            if (args.Length > 0 && int.TryParse(args[0], out numberOfProducers) && numberOfProducers > 0)
            {
                return numberOfProducers;
            }
            else if (args.Length > 0)
            {
                Console.WriteLine("The only provided argument should be a valid integer");
                return InvalidNumberOfProducers;
            }

            while (numberOfProducers <= 0)
            {
                Console.Write("Please enter the number of producers or 0 to exit: ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out numberOfProducers))
                {
                    if (numberOfProducers == 0)
                    {
                        return ExitInput;
                    }
                    else if (numberOfProducers > 0)
                    {
                        return numberOfProducers;
                    }
                }

                Console.WriteLine("This is not a valid integer!");
            }

            return InvalidNumberOfProducers;
        }
    }
}
