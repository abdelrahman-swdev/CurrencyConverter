using System;
using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Service.DTOs
{
    public class GetMostOrLeastNImprovedCurrenciesByDateParamsDto
    {
        public string From { get; set; }

        public string To { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Count must be greater than or equal to 1.")]
        public int Count { get; set; }
    }
}
