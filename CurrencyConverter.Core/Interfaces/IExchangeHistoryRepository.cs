﻿using CurrencyConverter.Core.Entities;
using System.Threading.Tasks;

namespace CurrencyConverter.Core.Interfaces
{
    public interface IExchangeHistoryRepository : IGenericRepository<ExchangeHistory>
    {
        // get latest history for currency
        Task<ExchangeHistory> GetLatestHistoryForCurrencyAsync(int curId);

        // get latest rate for currency
        Task<float> GetLatestRateForCurrencyAsync(int curId);
    }
}
