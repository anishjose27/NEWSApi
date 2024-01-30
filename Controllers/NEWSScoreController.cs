using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace NEWSApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NEWSScoreController : ControllerBase
    {
        private readonly ILogger<NEWSScoreController> _logger;
        private readonly IConfiguration _configuration;
        private readonly List<Type> _validTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NEWSScoreController"/> class.
        /// </summary>
        /// <param name="logger">Logger for logging messages.</param>
        /// <param name="configuration">Configuration for accessing application settings.</param>
        public NEWSScoreController(ILogger<NEWSScoreController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _validTypes = GetValidTypes();
        }

        /// <summary>
        /// Calculates the NEWS score based on the provided measurement data.
        /// </summary>
        /// <param name="inputData">Request body containing a list of measurements.</param>
        /// <returns>HTTP response with the calculated NEWS score or an error message.</returns>
        [HttpPost]
        [Route("Calculate")]
        public IActionResult CalculateNewsScore([FromBody] MeasurementRequest inputData)
        {
            try
            {
                //Validate the input for possible errors
                ValidateInputData(inputData.Measurements, _validTypes);
                //Call the calculate logic 
                var newsScore = CalculateScore(inputData.Measurements);
                return Ok(new { score = newsScore });
            }
            catch (ArgumentException ex)
            {
                // Log the handled error details
                _logger.LogError(ex, ex.Message);

                // Return a BadRequest response with the relevent error message
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Log other unexpected errors
                _logger.LogError(ex, ex.Message);

                // Return an Internal server error response with the corresponding error message
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Calculates the NEWS score based on a list of measurements and predefined type and range data.
        /// </summary>
        /// <param name="inputData">List of measurements for which NEWS score needs to be calculated.</param>
        /// <returns>Calculated NEWS score.</returns>
        private int CalculateScore(List<Measurement> inputData)
        {
            //Initialize the score as 0
            var newsScore = 0;

            //Iteratre through each input for calculation
            foreach (var measurement in inputData)
            {
                //Select the type to cross check the ranges
                var type = _validTypes.Find(p => p.Name.ToUpper() == measurement.Type.ToUpper());

                //Check if the measurement from the input falls between the lower/upper limit
                if (!(type.MinValue < measurement.Value && measurement.Value <= type.MaxValue))
                    throw new ArgumentException($"Value of {type.Description}({type.Name}) is out of valid range {type.MinValue}(Exclusive)-{type.MaxValue}(Inclusive)");

                //If all the validations are passed, find the range in which the input falls into
                var item = type.Ranges.Find(p => p.Contains(measurement.Value));
                if (item != null)
                {
                    //If range is found, add the corresponding value to the score
                    newsScore += item.Value;
                }
                else
                {
                    //If rage is not found, show the relevent error.
                    throw new ConfigurationErrorsException($"The value {measurement.Value} for {type.Description}({type.Name}) is not found in any of the defined ranges");
                }
            }
            return newsScore;

        }

        /// <summary>
        /// Validates the input data against the predefined types and performs various checks.
        /// </summary>
        /// <param name="inputData">List of measurements to be validated.</param>
        /// <param name="validTypes">List of valid measurement types with associated ranges.</param>
        private static void ValidateInputData(List<Measurement> inputData, List<Type> validTypes)
        {
            var validTypeNames = validTypes.Select(type => type.Name.ToUpper());
            var inputTypeNames = inputData.Select(p => p.Type.ToUpper());

            // Check if the lists have the same elements regardless of order
            bool areEqual = validTypeNames.Count() == inputTypeNames.Count() && validTypeNames.All(inputTypeNames.Contains);

            //Proceed to detailed validations if the types are not the same
            if (!areEqual)
            {

                //Check if all the required types are present in the input
                var missingTypes = validTypeNames.Except(inputTypeNames);
                if (missingTypes.Any())
                {
                    var errorMessage = new StringBuilder();
                    foreach (var type in missingTypes)
                    {
                        errorMessage.Append($"Measurement for {type} missing in input. ");
                    }
                    throw new ArgumentException(errorMessage.ToString());
                }

                //Check if any additional types are present in the input which are nor expected
                var additionalTypes = inputTypeNames.Except(validTypeNames);
                if (additionalTypes.Any())
                {
                    var errorMessage = new StringBuilder();
                    foreach (var type in additionalTypes)
                    {
                        errorMessage.Append($"Invalid type: {type} present in input. ");
                    }
                    throw new ArgumentException(errorMessage.ToString());
                }

                // Check for duplicates in the input
                var duplicates = inputTypeNames
                    .GroupBy(name => name)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToList();
                if (duplicates.Any())
                {
                    var errorMessage = new StringBuilder();
                    foreach (var type in duplicates)
                    {
                        errorMessage.Append($"Measurement for {type} is duplicated in input. ");
                    }
                    throw new ArgumentException(errorMessage.ToString());
                }
            }


        }

        /// <summary>
        /// Retrieves the valid types and their ranges based on the configuration file.
        /// </summary>
        /// <returns>A list of valid types with associated ranges.</returns>
        private List<Type> GetValidTypes()
        {
            var validTypes = new List<Type>();
            try
            {
                // Read the type data and range details based on the config files.
                var path = _configuration["FilePath"];

                if (string.IsNullOrEmpty(path))
                {
                    // Throw an exception if the path to the type data file is missing.
                    throw new ConfigurationErrorsException($"Unable to find the path to type data file. Please validate the configuration");
                }

                string jsonString = System.IO.File.ReadAllText(path);

                // Deserialize the JSON data to list
                validTypes = JsonSerializer.Deserialize<List<Type>>(jsonString);

                // Check if the list is empty
                if (validTypes == null || validTypes.Count == 0)
                {
                    // Throw an exception if valid types are not found in the type data file.
                    throw new ConfigurationErrorsException($"Valid types not found in the type data file. Please validate the configuration");
                }
            }
            catch (Exception ex)
            {
                // Log the handled error details
                _logger.LogError(ex, ex.Message);

                // Throw a ConfigurationErrorsException with the relevant error message.
                throw new ConfigurationErrorsException(ex.Message);
            }
            return validTypes;
        }


    }
}