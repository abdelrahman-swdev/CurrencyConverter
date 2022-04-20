using CurrencyConverter.Api.Errors;
using CurrencyConverter.Service.DTOs;
using CurrencyConverter.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CurrencyConverter.Api.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserToReturnDto>> Register(RegisterDto registerDto)
        {
            if (await _authService.CheckEmailExistsAsync(registerDto.Email))
            {
                return new BadRequestObjectResult(
                    new ApiValidationErrorResponse()
                    {
                        Errors = new[] {
                            "Email address is in use."
                        }
                    });
            }
            var result = await _authService.RegisterAsync(registerDto);
            if(result == null) return BadRequest(new ApiResponse(400));
            return Ok(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserToReturnDto>> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (result == null) return Unauthorized(new ApiResponse(401));
            return Ok(result);
        }
    }
}
