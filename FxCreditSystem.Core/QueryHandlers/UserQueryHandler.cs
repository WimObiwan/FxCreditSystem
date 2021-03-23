using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FxCreditSystem.Common;

namespace FxCreditSystem.Core
{
    public class UserQueryHandler : IUserQueryHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthorizationService _authorizationService;

        public async Task<IList<Common.Entities.AccountUser>> GetAccounts(string identity, Guid userId)
        {
            if (!await _authorizationService.CheckAuthorizedUser(identity, userId))
                throw new UnauthorizedAccessException();
            
            return await _userRepository.GetAccounts(userId);
        }

        public UserQueryHandler(IAuthorizationService authorizationService, IUserRepository userRepository) 
        {
            _authorizationService = authorizationService;
            _userRepository = userRepository;
        }
    }
}
