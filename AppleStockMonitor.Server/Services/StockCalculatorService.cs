namespace AppleStockMonitor.Server.Services
{
    public static class StockCalculatorService
    {
        /// <summary>
        /// Calculates the log returns for a given list of stock prices.
        /// </summary>
        /// <remarks>
        /// The first return is calculated as the log of the first price,
        /// and subsequent returns are calculated as the difference between
        /// the log of the current price and the log of the previous price.
        /// </remarks>
        /// <param name="dataset">The list of stock prices to process</param>
        /// <returns>The calculated list of return values</returns>
        private static List<double> CalculateReturn(List<double> dataset)
        // Method is private since it is only used by other methods within the same class
        {
            List<double> returns = new List<double>();

            returns.Add(Math.Log(dataset[0])); //Starting point has no precedent so it is kept at ln(r1)

            // Loop through the remaining dataset items to calculate the log return
            for (int i = 1; i < dataset.Count; i++)
            {
                double logReturn = Math.Log(dataset[i]) - Math.Log(dataset[i - 1]);
                returns.Add(logReturn);
            }

            return returns;
        }

        /// <summary>
        /// Calculates the arithmetic mean of the returns derived from a sequence of data points.
        /// </summary>
        /// <remarks>
        /// The method first computes the sequence of returns from the provided data points, then
        /// calculates the mean of those returns. The returns are calculated using the 
        /// <seealso cref="CalculateReturn"/> method.
        /// </remarks>
        /// <param name="data">The list of stock price data points from which to calculate returns.
        /// Cannot be null or empty.</param>
        /// <returns>The arithmetic mean of the calculated returns from the input data.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="data"/> is empty.</exception>
        public static double Mean(List<double> data)
        {
            // Validate input
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data list cannot be null.");
            }
            if (data.Count == 0)
            {
                throw new ArgumentException("Data list cannot be empty.");
            }

            // Calculate returns and return the average
            List<double> returns = CalculateReturn(data);

            return returns.Average();
        }

        /// <summary>
        /// Calculates the standard deviation of the list of stock prices.
        /// </summary>
        /// <remarks>
        /// The method, similar to the <seealso cref="Mean"/> method, first computes the sequence of returns
        /// from the provided data points, then calculates the standard deviation of those returns.
        /// The returns are calculated using the <seealso cref="CalculateReturn"/> method.
        /// The standard deviation is calculated using the formula: SQRT(SUM((x - mean)^2) / N),
        /// where x is each return, mean is the average return, and N is the number of returns.
        /// </remarks>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static double StandardDeviation(List<double> data)
        {
            // Validate input
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data list cannot be null.");
            }
            if (data.Count == 0)
            {
                throw new ArgumentException("Data list cannot be empty.");
            }

            // Calculate log returns and mean value
            List<double> returns = CalculateReturn(data);
            double mean = returns.Average();

            // Calculate and return the standard deviation
            double sumOfSquares = returns.Select(r => Math.Pow(r - mean, 2)).Sum();
            return Math.Sqrt(sumOfSquares / returns.Count);
        }
    }
}
