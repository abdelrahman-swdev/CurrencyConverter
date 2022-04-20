using CurrencyConverter.Api.Errors;
using CurrencyConverter.Core.Entities;
using CurrencyConverter.Service.DTOs;
using CurrencyConverter.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyConverter.Api.Controllers
{
    public class CurrencyController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Currency>>> GetAllCurrencies()
        {
            return Ok(await _currencyService.ListAllAsync());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddCurrency(CurrencyForCreationDto currencyDto)
        {
            await _currencyService.AddAsync(currencyDto);
            return Ok();
        }

        [HttpDelete("{currencyId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCurrency([FromRoute] int currencyId)
        {
            var result = await _currencyService.DeleteAsync(currencyId);
            if(result == 0) return NotFound(new ApiResponse(404));
            return NoContent();
        }

        [HttpGet("getbyname")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Currency>> GetCurrencyByName([FromQuery] string name)
        {
            var currency = await _currencyService.GetCurrencyByNameAsync(name);
            if (currency == null) return NotFound(new ApiResponse(404));
            return Ok(currency);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CurrencyForUpdateDto>> UpdateCurrency([FromBody] CurrencyForUpdateDto currencyDto)
        {
            var result = await _currencyService.UpdateAsync(currencyDto);
            if(result == null) return NotFound(new ApiResponse(404));
            return Ok(result);
        }
    }
}
