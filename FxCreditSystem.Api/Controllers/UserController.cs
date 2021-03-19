using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FxCreditSystem.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FxCreditSystem.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    //[Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserQueryHandler _userQueryHandler;

        public UserController(ILogger<UserController> logger, IUserQueryHandler userQueryHandler)
        {
            _logger = logger;
            _userQueryHandler = userQueryHandler;
        }

        [HttpGet]
        [Route("{id}/accounts")]
        public async Task<IActionResult> Get(string authUserId)
        {
            var result = await _userQueryHandler.GetAccounts(authUserId);
            return Ok(result);
        }
    }
}
