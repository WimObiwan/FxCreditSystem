using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FxCreditSystem.Common;
using FxCreditSystem.Common.Entities;

namespace FxCreditSystem.Core
{
    public class AccountQueryHandler : IAccountQueryHandler
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthorizationService _authorizationService;

        public async Task<Account> GetAccount(string identity, Guid accountId)
        {
            if (!await _authorizationService.CheckAuthorizedAccount(identity, accountId, AccessType.Read))
                throw new UnauthorizedAccessException();
            
            return await _accountRepository.GetAccount(accountId);
        }

        public AccountQueryHandler(IAuthorizationService authorizationService, IAccountRepository accountRepository) 
        {
            _authorizationService = authorizationService;
            _accountRepository = accountRepository;
        }
    }
}
