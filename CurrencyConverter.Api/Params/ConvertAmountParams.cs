using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Api.Params
{
    public class ConvertAmountParams
    {
        [Required]
        public string FromCurrency { get; set; }
        [Required]
        public string ToCurrency { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "Amount must be greater than or equal to 1.")]
        public float Amount { get; set; }
    }
}
