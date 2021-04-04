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
    /// <summary>
    ///   Endpoint for Account interactions
    /// </summary>
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private IIdentityRetriever _identityRetriever;
        private readonly IAccountQueryHandler _accountQueryHandler;

        /// <summary>
        ///   Endpoint for User interactions
        /// </summary>
        public AccountController(ILogger<UserController> logger, IMapper mapper, IIdentityRetriever identityRetriever, IAccountQueryHandler accountQueryHandler)
        {
            _logger = logger;
            _mapper = mapper;
            _identityRetriever = identityRetriever;
            _accountQueryHandler = accountQueryHandler;
        }

        /// <summary>
        ///     Gets identities of a user
        /// </summary>       
        [HttpGet]
        [Route("{accountId}")]
        public async Task<ActionResult<AccountResponse>> GetAccount(Guid accountId)
        {
            var identity = _identityRetriever.GetIdentity(this);
            var result = await _accountQueryHandler.GetAccount(identity, accountId);
            return Ok(_mapper.Map<AccountResponse>(result));
        }

        // /// <summary>
        // ///     Gets accounts of a user
        // /// </summary>       
        // [HttpGet]
        // [Route("{userId}/accounts")]
        // public async Task<ActionResult<IList<AccountUserResponse>>> GetAccounts(Guid userId)
        // {
        //     var identity = _identityRetriever.GetIdentity(this);
        //     var result = await _accountQueryHandler.GetAccounts(identity, userId);
        //     return Ok(_mapper.Map<IList<AccountUserResponse>>(result));
        // }
    }
}
