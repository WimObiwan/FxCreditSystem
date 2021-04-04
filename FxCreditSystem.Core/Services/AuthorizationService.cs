using System;
using System.Threading.Tasks;
using FxCreditSystem.Common;

namespace FxCreditSystem.Core
{
    public interface IAuthorizationService
    {
        Task<bool> CheckAuthorizedUser(string identity, Guid userId);
        Task<bool> CheckAuthorizedAccount(string identity, Guid accountId);
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

        public async Task<bool> CheckAuthorizedUser(string identity, Guid userId)
        {
            if (await _userRepository.HasIdentity(userId, identity))
                return true;

            // TODO: Implement "admin" access

            return false;
        }

        public async Task<bool> CheckAuthorizedAccount(string identity, Guid accountId)
        {
            if (await _accountRepository.HasIdentity(accountId, identity))
                return true;

            // TODO: Implement "admin" access

            return false;
        }
    }
}