using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Service.DTOs
{
    public class CurrencyForCreationDto
    {

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Sign { get; set; }

        [Required, Display(Name = "Value Against Dollar")]
        [Range(0.00001, float.MaxValue, ErrorMessage = "The field Value Against Dollar must be greater than zero.")]
        public float ValueAgainstUsd { get; set; }
    }
}
