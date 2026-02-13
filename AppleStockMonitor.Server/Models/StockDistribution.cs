namespace AppleStockMonitor.Server.Models
{
    /// <summary>
    /// Enumerator to ensure that the interval is one of the specified values
    /// (Daily, Weekly, Monthly) for better type safety and code readability.
    /// </summary>
    public enum Interval
    {
        Daily,
        Weekly,
        Monthly
    }

    /// <summary>
    /// Represents the statistical distribution results for stock data over a specified date range and interval.
    /// </summary>
    /// <remarks>
    /// This class encapsulates the mean and standard deviation of stock values within a given
    /// interval and date range. It can be used to model or analyze the distribution of stock prices or returns for
    /// financial analysis and reporting.
    /// </remarks>
    public class StockDistribution
    {
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public Interval Interval { get; set; }
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }
    }
}
