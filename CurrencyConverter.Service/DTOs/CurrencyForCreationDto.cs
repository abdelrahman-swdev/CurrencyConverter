using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Service.DTOs
{
    public class CurrencyForCreationDto
    {

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Sign { get; set; }

        [Required]
        public float Rate { get; set; }
    }
}
