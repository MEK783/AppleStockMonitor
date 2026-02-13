namespace AppleStockMonitor.Server.Models
{
    /// <summary>
    /// Represents a request to retrieve distribution data for a specified date range and interval.
    /// </summary>
    public class DistributionRequest
    {
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public Interval Interval { get; set; }
    }
}
