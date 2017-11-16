using FibonacciLib;
using System.Diagnostics;
using System.Linq;
using static System.Console;

namespace Fibonacci
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Starting Fibonacci example...");

            var fibonacciCalculator = FibonacciCalculatorFactory.Create();

            int position;
            if (!args.Any() || !int.TryParse(args.First(), out position) || position < 0)
            {
                position = 40;
            }

            WriteLine($"For this test, we will calculate the {position}th position!");
            WriteLine();

            Iterative(fibonacciCalculator, position);
            WriteLine();

            Recursive(fibonacciCalculator, position);
            WriteLine();

            Asynchronous(fibonacciCalculator, position);
            WriteLine();

            WriteLine("Press any key to exit...");
            ReadKey();
        }

        private static void Iterative(IFibonacciCalculator calculator, int position)
        {
            WriteLine("Starting the iterative calculation...");

            var clock = Stopwatch.StartNew();
            var iterativeResult = calculator.IterativeCalculation(position);
            clock.Stop();

            WriteLine($"The iterative result is {iterativeResult} and the calculation took {clock.ElapsedMilliseconds}ms");
        }

        private static void Recursive(IFibonacciCalculator calculator, int position)
        {
            WriteLine("Starting the recursive calculation...");

            var clock = Stopwatch.StartNew();
            var recursiveResult = calculator.RecursiveCalculation(position);
            clock.Stop();

            WriteLine($"The recursive result is {recursiveResult} and the calculation took {clock.ElapsedMilliseconds}ms");
        }

        private static void Asynchronous(IFibonacciCalculator calculator, int position)
        {
            WriteLine("Starting the asynchronous calculation...");

            var clock = Stopwatch.StartNew();
            var asyncResult = calculator.AsyncCalculation(position);

            WriteLine("Waiting for the result...");
            asyncResult.Wait();
            clock.Stop();

            WriteLine($"The asynchronous result is {asyncResult.Result} and the calculation took {clock.ElapsedMilliseconds}ms");
        }
    }
}
