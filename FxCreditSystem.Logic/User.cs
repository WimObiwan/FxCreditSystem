using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FxCreditSystem.Common;

namespace FxCreditSystem.Logic
{
    public class User
    {
        private readonly IAccountUserRepository accountUserRepository;
        
        public async Task<IList<Common.Entities.AccountUser>> GetAccountsForUser(string authUserId)
        {
            return await accountUserRepository.Get(authUserId);
        }

        public User(IAccountUserRepository accountUserRepository) 
        {
            this.accountUserRepository = accountUserRepository;
        }
    }
}
