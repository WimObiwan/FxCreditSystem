using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FxCreditSystem.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    //[Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("{id}")]
        public AccountResponse Get(Guid id)
        {
            var rng = new Random();
            return new AccountResponse() 
            {
                Id = id,
                Credits = (decimal)rng.NextDouble()
            };
        }
    }
}
