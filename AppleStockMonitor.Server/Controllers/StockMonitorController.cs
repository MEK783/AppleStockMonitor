using AppleStockMonitor.Server.Models;
using AppleStockMonitor.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AppleStockMonitor.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockMonitorController : Controller
    {
        /// <summary>
        /// Retrieves the stock price distribution for a specified date range and interval using the Polygon API.
        /// </summary>
        /// <remarks>This method validates the input request and communicates with the Polygon API to
        /// retrieve stock price data. If the request contains invalid data, a 400 Bad Request response is returned. If
        /// an error occurs during processing or communication with the external API, a 500 Internal Server Error
        /// response is returned. The method does not perform detailed validation beyond date and interval checks;
        /// additional validation is expected to be handled by the frontend.</remarks>
        /// <param name="request">The request parameters specifying the date range and interval for the stock distribution calculation. The
        /// <paramref name="request"/> must have a valid interval and the <c>FromDate</c> must be earlier than
        /// <c>ToDate</c>.</param>
        /// <returns>An HTTP response containing the calculated stock distribution, including mean and standard deviation, for
        /// the specified range and interval. Returns status code 200 with the distribution data if successful;
        /// otherwise, returns status code 400 or 500 with an error message.</returns>
        [HttpPost("distribution")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> GetDistribution([FromBody] DistributionRequest request)
        {
            try
            {
                // Validate the request data
                if (request.FromDate > request.ToDate)
                {
                    throw new BadHttpRequestException("FromDate must be earlier than ToDate.");
                    // No further validation needed as the rest of the validation will be handled by the Frontend
                }

                // Prepare the API request to the Polygon API
                string interval;
                switch (request.Interval)
                {
                    case Interval.Daily:
                        interval = "day";
                        break;
                    case Interval.Weekly:
                        interval = "week";
                        break;
                    case Interval.Monthly:
                        interval = "month";
                        break;
                    default: // Added the default case if bad data is provided, even though it cannot
                        throw new BadHttpRequestException("Invalid interval specified.");
                }
                string from = request.FromDate.ToString("yyyy-MM-dd");
                string to = request.ToDate.ToString("yyyy-MM-dd");
                // Geth the API key and base URL from the application configuration
                string apiKey = AppContext.GetData("PolygonApiKey")?.ToString() ?? throw new Exception("Polygon API key is not configured.");
                string baseUrl = AppContext.GetData("PolygonApiBaseUrl")?.ToString() ?? throw new Exception("Polygon API base URL is not configured.");

                string url = $"{baseUrl}/{interval}/{from}/{to}?apiKey={apiKey}";

                // Connect to the Polygon API and retrieve the stock price data for the specified request
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                // Send request to the Polygon API and get the response
                var response = await client.GetFromJsonAsync<dynamic>(url).Result;

                // Check if the response threw an error
                if (response.status == "ERROR")
                {
                    throw new Exception($"Polygon API error: {response.error}");
                }

                // Process the response and return the stock distribution data
                List<double> prices = ((JsonArray)response.results).Select(p => (double)p["c"]).ToList();

                return Ok(new StockDistribution
                {
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    Interval = request.Interval,
                    Mean = StockCalculatorService.Mean(prices),
                    StandardDeviation = StockCalculatorService.StandardDeviation(prices)
                });

            }
            catch (BadHttpRequestException ex)
            {
                // Return a 400 Bad Request with the exception message
                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    $"Request has invalid data: {ex.Message}"
                );
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error with the exception message
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    $"An error occurred while processing the request: {ex.Message}"
                );
            }
        }

        /// <summary>
        /// Returns a list of available intervals
        /// </summary>
        /// <remarks>
        /// This method retrieves the list of available intervals for stock price distribution calculations.
        /// The intervals are defined in the <c>Interval</c> enum and include Daily, Weekly, and Monthly options.
        /// This endpoint is used to ensure that only valid interval values are provided.
        /// </remarks>
        /// <returns>A json response containing the list of valid interval values</returns>
        [HttpGet("intervals")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult GetIntervals()
        {
            // Return the list of intervals as a JSON response
            var intervals = Enum.GetNames(typeof(Interval)).ToList();
            return Ok(intervals);
        }

    }
}
