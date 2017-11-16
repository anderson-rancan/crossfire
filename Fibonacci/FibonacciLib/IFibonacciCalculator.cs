using System.Threading.Tasks;

namespace FibonacciLib
{
    public interface IFibonacciCalculator
    {
        double IterativeCalculation(int position);

        double RecursiveCalculation(int position);

        Task<double> AsyncCalculation(int position);
    }
}
