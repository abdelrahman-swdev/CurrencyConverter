namespace CurrencyConverter.Service.DTOs
{
    public class CurrencyWithDecreasedRateToReturnDto : CurrencyToReturnDto
    {
        public float DecreasedAmount { get; set; }
    }
}
