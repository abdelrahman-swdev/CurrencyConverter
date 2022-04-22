using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Api.Params
{
    public class GetMostOrLeastNImprovedCurrenciesByDateParams
    {
        [Required]
        public string From { get; set; }
        [Required]

        public string To { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 1.")]
        public int Count { get; set; }
    }
}
