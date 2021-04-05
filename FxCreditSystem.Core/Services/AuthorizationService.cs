using System;
using System.Threading.Tasks;
using FxCreditSystem.Common;

namespace FxCreditSystem.Core
{
    public interface IAuthorizationService
    {
        Task<bool> CheckAuthorizedUser(string identity, Guid userId, AccessType accessType);
        Task<bool> CheckAuthorizedAccount(string identity, Guid accountId, AccessType accessType);
    }

    [Flags]
    public enum AccessType
    {
        Read = 1,
        Write = 2,
        Any = int.MaxValue,
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
            if (await _userRepository.HasIdentity(userId, identity))
                return true;

            // TODO: Implement "admin" access

            return false;
        }

        public async Task<bool> CheckAuthorizedAccount(string identity, Guid accountId, AccessType accessType)
        {
            if (await _accountRepository.HasIdentity(accountId, identity))
                return true;

            // TODO: Implement "admin" access

            return false;
        }
    }
}