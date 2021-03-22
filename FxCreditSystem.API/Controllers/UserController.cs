using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FxCreditSystem.API.DTO;
using FxCreditSystem.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FxCreditSystem.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserQueryHandler _userQueryHandler;

        public UserController(ILogger<UserController> logger, IMapper mapper, IUserQueryHandler userQueryHandler)
        {
            _logger = logger;
            _mapper = mapper;
            _userQueryHandler = userQueryHandler;
        }

        [HttpGet]
        [Route("{authUserId}/accounts")]
        public async Task<IActionResult> Get(string authUserId)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier);
            var result = await _userQueryHandler.GetAccounts(authUserId);
            return Ok(_mapper.Map<IList<AccountUserResponse>>(result));
        }
    }
}
