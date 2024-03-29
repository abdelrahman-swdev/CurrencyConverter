﻿using CurrencyConverter.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Api.Controllers
{
    [Route("errors/{code}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : BaseApiController
    {
        public IActionResult Error(int code) =>
             new ObjectResult(new ApiResponse(code));
    }
}
