using System;

namespace CurrencyConverter.Core.Entities
{
    public class ExchangeHistory : BaseEntity
    {
        public DateTime ExchangeDate { get; set; }

        public float Rate { get; set; }

        public int CurId { get; set; }
        public Currency Currency { get; set; }
    }
}
