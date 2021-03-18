using System.Collections.Generic;
using System.Threading.Tasks;
using FxCreditSystem.Common;

namespace FxCreditSystem.Core
{
    public class UserQueryHandler
    {
        private readonly IAccountUserRepository accountUserRepository;
        
        public async Task<IList<Common.Entities.AccountUser>> GetAccountsForUser(string authUserId)
        {
            return await accountUserRepository.Get(authUserId);
        }

        public UserQueryHandler(IAccountUserRepository accountUserRepository) 
        {
            this.accountUserRepository = accountUserRepository;
        }
    }
}
