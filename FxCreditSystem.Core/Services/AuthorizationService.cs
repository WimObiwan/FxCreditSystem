using System;
using System.Threading.Tasks;
using FxCreditSystem.Common;

namespace FxCreditSystem.Core
{
    public interface IAuthorizationService
    {
        Task<bool> CheckAuthorizedUser(string identity, Guid userId);
    }

    public class AuthorizationService : IAuthorizationService
    {
        IUserRepository _userRepository;

        public AuthorizationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> CheckAuthorizedUser(string identity, Guid userId)
        {
            if (await _userRepository.HasIdentity(userId, identity))
                return true;

            // TODO: Implement "admin" access

            return false;
        }
    }
}