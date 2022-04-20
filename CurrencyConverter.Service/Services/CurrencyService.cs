using CurrencyConverter.Core.Entities;
using CurrencyConverter.Core.Interfaces;
using CurrencyConverter.Service.DTOs;
using CurrencyConverter.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Service.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeHistoryRepository _exchangeHistoryRepository;

        public CurrencyService(ICurrencyRepository currencyRepository, IExchangeHistoryRepository exchangeHistoryRepository)
        {
            _currencyRepository = currencyRepository;
            _exchangeHistoryRepository = exchangeHistoryRepository;
        }
        public async Task AddAsync(CurrencyForCreationDto currencyDto)
        {
            var currency = new Currency
            {
                IsActive = true,
                Name = currencyDto.Name,
                Sign = currencyDto.Sign
            };
            await _currencyRepository.AddAsync(currency);

            var exchangeHistory = new ExchangeHistory
            {
                CurId = currency.Id,
                ExchangeDate = DateTime.Now,
                Rate = currencyDto.Rate
            };
            // create _exchangeHistoryRepository
            await _exchangeHistoryRepository.AddAsync(exchangeHistory);
        }

        public async Task<int> DeleteAsync(int currencyId)
        {
            var currency = await _currencyRepository.FindByIdAsync(currencyId);
            if (currency == null) return 0;
            return await _currencyRepository.DeleteAsync(currency);
        }

        public async Task<Currency> FindByIdAsync(int id)
        {
            return await _currencyRepository.FindByIdAsync(id);
        }

        public async Task<Currency> GetCurrencyByNameAsync(string name)
        {
            return await _currencyRepository.GetCurrencyByNameAsync(name);
        }

        public async Task<IReadOnlyList<Currency>> ListAllAsync()
        {
            return await _currencyRepository.ListAllAsync();
        }

        /// <summary>
        /// update the entity
        /// </summary>
        /// <param name="currencyDto"></param>
        /// <returns>entity or null</returns>
        public async Task<CurrencyForUpdateDto> UpdateAsync(CurrencyForUpdateDto currencyDto)
        {
            var currency = await _currencyRepository.FindByIdAsync(currencyDto.Id);
            if (currency == null) return null;

            currency.Name = currencyDto.Name;
            currency.Sign = currencyDto.Sign;

            // get latest history for this currency
            // to know if rate changed or not
            var latestHistory = await _exchangeHistoryRepository.GetLatestHistoryForCurrencyAsync(currency.Id);
            if (latestHistory != null)
            {
                if (latestHistory.Rate != currencyDto.Rate)
                {
                    // if changed add a record in exchangeHistory table with new rate for this currency
                    var newHistory = new ExchangeHistory
                    {
                        CurId = currency.Id,
                        ExchangeDate = DateTime.Now,
                        Rate = currencyDto.Rate
                    };
                    await _exchangeHistoryRepository.AddAsync(newHistory);
                }
            }
            await _currencyRepository.UpdateAsync(currency);
            return currencyDto;
        }
    }
}
