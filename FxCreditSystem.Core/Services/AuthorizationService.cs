using System;
using System.Threading.Tasks;
using FxCreditSystem.Common;
using FxCreditSystem.Common.Entities;

namespace FxCreditSystem.Core
{
    public interface IAuthorizationService
    {
        Task<bool> CheckAuthorizedUser(string identity, Guid userId, AccessType accessType);
        Task<bool> CheckAuthorizedAccount(string identity, Guid accountId, AccessType accessType);
    }

    public class AuthorizationService : IAuthorizationService
    {
        IUserRepository _userRepository;
        IAccountRepository _accountRepository;

        public AuthorizationService(IUserRepository userRepository, IAccountRepository accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }

        public async Task<bool> CheckAuthorizedUser(string identity, Guid userId, AccessType accessType)
        {
            if (await _userRepository.CheckIdentityScope(identity, userId))
                return true;

            // TODO: Implement "admin" access

            return false;
        }

        public async Task<bool> CheckAuthorizedAccount(string identity, Guid accountId, AccessType accessType)
        {
            if (await _accountRepository.CheckIdentity(accountId, identity))
                return true;

            // TODO: Implement "admin" access

            return false;
        }
    }
}