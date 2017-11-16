namespace FibonacciLib
{
    public static class FibonacciCalculatorFactory
    {
        public static IFibonacciCalculator Create()
        {
            return new FibonacciCalculator();
        }
    }
}
