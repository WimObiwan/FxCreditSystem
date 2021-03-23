using System;
using System.Threading.Tasks;

namespace FxCreditSystem.Core
{
    public interface IAuthorizationService
    {
        Task<bool> CheckAuthorizedUser(string identity, Guid userId);
    }

    public interface IUserIdentityRepository
    {
        Task<bool> UserHasIdentity(Guid userId, string identity);
    }

    public class AuthorizationService : IAuthorizationService
    {
        IUserIdentityRepository _userIdentityRepository;

        public AuthorizationService(IUserIdentityRepository userIdentityRepository)
        {
            _userIdentityRepository = userIdentityRepository;
        }

        public async Task<bool> CheckAuthorizedUser(string identity, Guid userId)
        {
            if (await _userIdentityRepository.UserHasIdentity(userId, identity))
                return true;

            // TODO: Implement "admin" access

            return false;
        }
    }
}