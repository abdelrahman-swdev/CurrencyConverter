using CurrencyConverter.Api.Errors;
using CurrencyConverter.Api.Params;
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

        [HttpGet("GetAllCurrencies")]
        public async Task<ActionResult<IReadOnlyList<CurrencyToReturnDto>>> GetAllCurrencies() => 
            Ok(await _currencyService.ListAllAsync());
        

        [HttpPost("AddCurrency")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddCurrency(CurrencyForCreationDto currencyDto)
        {
            await _currencyService.AddAsync(currencyDto);
            return Ok();
        }

        [HttpDelete("DeleteCurrency/{currencyId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCurrency([FromRoute] int currencyId)
        {
            var result = await _currencyService.DeleteAsync(currencyId);
            return result == 0 ? NotFound(new ApiResponse(404)) : NoContent();
        }

        [HttpGet("GetCurrencyByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Currency>> GetCurrencyByName([FromQuery] string name)
        {
            var currencyToReturnDto = await _currencyService.GetCurrencyByNameAsync(name);
            return currencyToReturnDto == null ? NotFound(new ApiResponse(404)) : Ok(currencyToReturnDto);
        }

        [HttpPut("UpdateCurrency")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CurrencyForUpdateDto>> UpdateCurrency([FromBody] CurrencyForUpdateDto currencyDto)
        {
            var result = await _currencyService.UpdateAsync(currencyDto);
            return result == null ? NotFound(new ApiResponse(404)) : Ok(result);
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
            
            return Ok(await _currencyService.GetHighestNCurrenciesAsync(count));
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

            return Ok(await _currencyService.GetLowestNCurrenciesAsync(count));
        }

        [HttpPost("GetMostNImprovedCurrenciesByDate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CurrencyWithRateToReturnDto>> GetMostNImprovedCurrenciesByDate(
            [FromBody] GetMostOrLeastNImprovedCurrenciesByDateParams model)
        {
            DateTime fromDate, toDate;
            if (!DateTime.TryParse(model.From, out fromDate) || !DateTime.TryParse(model.To, out toDate))
                return new BadRequestObjectResult(new ApiValidationErrorResponse()
                {
                    Errors = new[] { "Invalid date format, enter something like 4/20/2022 3:35:23 AM" }
                }
            );

            var result = await _currencyService.GetMostNImprovedCurrenciesByDateAsync(fromDate, toDate, model.Count);
            return result.Count == 0 ? Ok("'No currencies improved within this date.'") : Ok(result);
        }

        [HttpPost("GetLeastNImprovedCurrenciesByDate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CurrencyWithRateToReturnDto>> GetLeastNImprovedCurrenciesByDate(
            [FromBody] GetMostOrLeastNImprovedCurrenciesByDateParams args)
        {
            DateTime fromDate, toDate;
            if (!DateTime.TryParse(args.From, out fromDate) || !DateTime.TryParse(args.To, out toDate))
                return new BadRequestObjectResult(new ApiValidationErrorResponse()
                {
                    Errors = new[] { "Invalid date format, enter something like 4/20/2022 3:35:23 AM" }
                }
            );

            var result = await _currencyService.GetLeastNImprovedCurrenciesByDateAsync(fromDate, toDate, args.Count);
            return result.Count == 0 ? Ok("'No currencies decreased within this date.'") : Ok(result);
        }

        [HttpPost("ConvertAmount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<float>> ConvertAmount(ConvertAmountParams args)
        {
            var result = await _currencyService.ConvertAmountAsync(args.FromCurrency, args.ToCurrency, args.Amount);
            return result == -1 ? NotFound(new ApiResponse(404)) : Ok(result);
        }
    }
}
