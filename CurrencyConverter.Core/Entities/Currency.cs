using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Core.Entities
{
    public class Currency : BaseEntity
    {
        public Currency()
        {
            ExchangeHistory = new List<ExchangeHistory>();
        }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Sign { get; set; }

        public bool IsActive { get; set; }

        public List<ExchangeHistory> ExchangeHistory { get; set; }
    }
}
