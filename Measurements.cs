using System.ComponentModel.DataAnnotations;

namespace NEWSApi
{
    /// <summary>
    /// Represents a measurement input with a type as string and a corresponding value as integer.
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// Gets or sets the type of the measurement.
        /// </summary>
        [Required(ErrorMessage = "Input parameter 'Type' is required.")]
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the value of the measurement.
        /// </summary>
        [Required(ErrorMessage = "Input parameter 'Value' is required.")]
        public int? Value { get; set; }
    }

    /// <summary>
    /// Represents a request containing a list of measurements.
    /// </summary>
    public class MeasurementRequest
    {
        /// <summary>
        /// Gets or sets the list of measurements in the request.
        /// </summary>
        [Required(ErrorMessage = "Please provide the measuremets required for the calculation.")]
        public List<Measurement>? Measurements { get; set; }
    }
}