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
        private IIdentityRetriever _identityRetriever;
        private readonly IUserQueryHandler _userQueryHandler;

        public UserController(ILogger<UserController> logger, IMapper mapper, IIdentityRetriever identityRetriever, IUserQueryHandler userQueryHandler)
        {
            _logger = logger;
            _mapper = mapper;
            _identityRetriever = identityRetriever;
            _userQueryHandler = userQueryHandler;
        }

        /// <summary>
        ///     Gets identities of a user
        /// </summary>       
        [HttpGet]
        [Route("{userId}/identities")]
        public async Task<IActionResult> GetIdentities(Guid userId)
        {
            var identity = _identityRetriever.GetIdentity(this);
            var result = await _userQueryHandler.GetIdentities(identity, userId);
            return Ok(_mapper.Map<IList<UserIdentityResponse>>(result));
        }

        /// <summary>
        ///     Gets accounts of a user
        /// </summary>       
        [HttpGet]
        [Route("{userId}/accounts")]
        public async Task<IActionResult> GetAccounts(Guid userId)
        {
            var identity = _identityRetriever.GetIdentity(this);
            var result = await _userQueryHandler.GetAccounts(identity, userId);
            return Ok(_mapper.Map<IList<AccountUserResponse>>(result));
        }
    }
}
