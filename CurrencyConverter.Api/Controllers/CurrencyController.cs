using CurrencyConverter.Api.Errors;
using CurrencyConverter.Core.Entities;
using CurrencyConverter.Service.DTOs;
using CurrencyConverter.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
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
        public async Task<ActionResult<IReadOnlyList<CurrencyToReturnDto>>> GetAllCurrencies() => 
            Ok(await _currencyService.ListAllAsync());
        

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
            var CurrencyToReturnDto = await _currencyService.GetCurrencyByNameAsync(name);
            if (CurrencyToReturnDto == null) return NotFound(new ApiResponse(404));
            return Ok(CurrencyToReturnDto);
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

        [HttpGet("GetHighestNCurrencies/{count}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CurrencyWithRateToReturnDto>> GetHighestNCurrencies([FromRoute] int count)
        {
            if(count <= 0) return new BadRequestObjectResult( new ApiValidationErrorResponse()
                {
                    Errors = new[] { "Count must be greater than 0" }
                }
            );

            var result = await _currencyService.GetHighestNCurrenciesAsync(count);
            return Ok(result);
        }

        [HttpGet("GetLowestNCurrencies/{count}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CurrencyWithRateToReturnDto>> GetLowestNCurrencies([FromRoute] int count)
        {
            if (count <= 0) return new BadRequestObjectResult(new ApiValidationErrorResponse()
            {
                Errors = new[] { "Count must be greater than 0" }
            });

            var result = await _currencyService.GetLowestNCurrenciesAsync(count);
            return Ok(result);
        }

        [HttpPost("GetMostNImprovedCurrenciesByDate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CurrencyWithRateToReturnDto>> GetMostNImprovedCurrenciesByDate(
            [FromBody] GetMostOrLeastNImprovedCurrenciesByDateParamsDto model)
        {
            DateTime fromDate, toDate;
            if (!DateTime.TryParse(model.From, out fromDate) || !DateTime.TryParse(model.To, out toDate))
                return new BadRequestObjectResult(new ApiValidationErrorResponse()
                {
                    Errors = new[] { "Invalid date format, enter something like 4/20/2022 3:35:23 AM" }
                }
            );

            var result = await _currencyService.GetMostNImprovedCurrenciesByDateAsync(fromDate, toDate, model.Count);
            return Ok(result);
        }

        [HttpPost("GetLeastNImprovedCurrenciesByDate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CurrencyWithRateToReturnDto>> GetLeastNImprovedCurrenciesByDate(
            [FromBody] GetMostOrLeastNImprovedCurrenciesByDateParamsDto model)
        {
            DateTime fromDate, toDate;
            if (!DateTime.TryParse(model.From, out fromDate) || !DateTime.TryParse(model.To, out toDate))
                return new BadRequestObjectResult(new ApiValidationErrorResponse()
                {
                    Errors = new[] { "Invalid date format, enter something like 4/20/2022 3:35:23 AM" }
                }
            );

            var result = await _currencyService.GetLeastNImprovedCurrenciesByDateAsync(fromDate, toDate, model.Count);
            return Ok(result);
        }
    }
}
