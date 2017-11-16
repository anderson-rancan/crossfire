using System.Threading.Tasks;

namespace FibonacciLib
{
    internal class FibonacciCalculator : IFibonacciCalculator
    {
        public double IterativeCalculation(int position)
        {
            var first = 0D;
            var second = 1D;
            var result = 0D;

            if (position < 2) return position;

            for (int counter = 2; counter <= position; counter++)
            {
                result = first + second;
                first = second;
                second = result;
            }

            return result;
        }

        public double RecursiveCalculation(int position)
        {
            if (position < 2) return position;

            return RecursiveCalculation(position - 1)
                + RecursiveCalculation(position - 2);
        }

        public async Task<double> AsyncCalculation(int position)
        {
            return await Task.Factory.StartNew(() => IterativeCalculation(position));
        }
    }
}
