using CurrencyConverter.Core.Entities;
using CurrencyConverter.Core.Interfaces;
using CurrencyConverter.Service.DTOs;
using CurrencyConverter.Service.Enums;
using CurrencyConverter.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyConverter.Service.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeHistoryRepository _exchangeHistoryRepository;
        private static readonly double _dollar = Convert.ToDouble(Dollar.One);

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
                Rate = (float)Math.Round(_dollar / currencyDto.ValueAgainstUsd, 3)
            };
            await _exchangeHistoryRepository.AddAsync(exchangeHistory);
        }

        public async Task<int> DeleteAsync(int currencyId)
        {
            var currency = await _currencyRepository.FindByIdAsync(currencyId);
            return currency == null ? 0 : await _currencyRepository.DeleteAsync(currency);
        }

        public async Task<Currency> FindByIdAsync(int id) => 
            await _currencyRepository.FindByIdAsync(id);
        

        public async Task<CurrencyToReturnDto> GetCurrencyByNameAsync(string name)
        {
            var currency = await _currencyRepository.GetCurrencyByNameAsync(name);
            return currency == null 
                ? null 
                : new CurrencyToReturnDto { Id = currency.Id, Sign = currency.Sign, Name = currency.Name };
        }

        public async Task<IReadOnlyList<CurrencyToReturnDto>> ListAllAsync()
        {
            var data = await _currencyRepository.ListAllAsync();
            return data.Select(c => 
                new CurrencyToReturnDto { Id = c.Id, Name = c.Name, Sign = c.Sign}).ToList();
        }

        /// <summary>
        /// update the entity
        /// </summary>
        /// <param name="currencyDto"></param>
        /// <returns>CurrencyForUpdateDto or null</returns>
        public async Task<CurrencyForUpdateDto> UpdateAsync(CurrencyForUpdateDto currencyDto)
        {
            var currency = await _currencyRepository.FindByIdAsync(currencyDto.Id);
            if (currency == null) return null;

            currency.Name = currencyDto.Name;
            currency.Sign = currencyDto.Sign;

            // get latest rate for this currency
            // to know if rate changed or not
            var latestHistoryRate = await _exchangeHistoryRepository.GetLatestRateForCurrencyAsync(currency.Id);
            var newRate = (float)Math.Round(_dollar / currencyDto.ValueAgainstUsd, 3);
            if (latestHistoryRate != newRate)
            {
                // if changed add a record in exchangeHistory table with new rate for this currency
                var newHistory = new ExchangeHistory
                {
                    CurId = currency.Id,
                    ExchangeDate = DateTime.Now,
                    Rate = newRate
                };
                await _exchangeHistoryRepository.AddAsync(newHistory);
            }
            await _currencyRepository.UpdateAsync(currency);
            return currencyDto;
        }

        public async Task<IReadOnlyList<CurrencyWithRateToReturnDto>> GetHighestNCurrenciesAsync(int count)
        {
            var result = await _currencyRepository.GetHighestNCurrenciesAsync(count);
            return result.Select(c => new CurrencyWithRateToReturnDto
            {
                Id = c.Key.Id,
                Name = c.Key.Name,
                Rate = c.Value,
                Sign = c.Key.Sign
            }).OrderByDescending(c => c.Rate).ToList();
        }

        public async Task<IReadOnlyList<CurrencyWithRateToReturnDto>> GetLowestNCurrenciesAsync(int count)
        {
            var result = await _currencyRepository.GetLowestNCurrenciesAsync(count);
            return result.Select(c => new CurrencyWithRateToReturnDto
            {
                Id = c.Key.Id,
                Name = c.Key.Name,
                Rate = c.Value,
                Sign = c.Key.Sign
            }).OrderBy(c => c.Rate).ToList();
        }

        public async Task<IReadOnlyList<CurrencyWithImprovedRateToReturnDto>> GetMostNImprovedCurrenciesByDateAsync(DateTime from, DateTime to, int count)
        {
            var result = await _currencyRepository.GetMostNImprovedCurrenciesByDateAsync(from, to, count);
            return result.Select(c => new CurrencyWithImprovedRateToReturnDto
            {
                Id = c.Key.Id,
                Name = c.Key.Name,
                ImprovedAmount = c.Value,
                Sign = c.Key.Sign
            }).OrderByDescending(c => c.ImprovedAmount).ToList();
        }

        public async Task<IReadOnlyList<CurrencyWithDecreasedRateToReturnDto>> GetLeastNImprovedCurrenciesByDateAsync(DateTime from, DateTime to, int count)
        {
            var result = await _currencyRepository.GetLeastNImprovedCurrenciesByDateAsync(from, to, count);
            return result.Select(c => new CurrencyWithDecreasedRateToReturnDto
            {
                Id = c.Key.Id,
                Name = c.Key.Name,
                DecreasedAmount = c.Value,
                Sign = c.Key.Sign
            }).OrderByDescending(c => c.DecreasedAmount).ToList();
        }


        /// <summary>
        /// convert amount from currency to another one 
        /// </summary>
        /// <param name="fromCurrencyName"></param>
        /// <param name="toCurrencyName"></param>
        /// <param name="amount"></param>
        /// <returns>converted amount or, -1 if on of currencies not found</returns>
        public async Task<float> ConvertAmountAsync(string fromCurrencyName, string toCurrencyName, float amount)
        {
            return await _currencyRepository.ConvertAmountAsync(fromCurrencyName, toCurrencyName, amount);
        }
    }
}
