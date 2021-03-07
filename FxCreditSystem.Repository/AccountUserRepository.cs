using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FxCreditSystem.Common;
using FxCreditSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository
{
    public class AccountUserRepository
    {
        private readonly DataContext dataContext;

        internal async Task<bool> Get(long accountId, string authUserId)
        {
            var accountUser = await dataContext.AccountUsers.Where(au => au.User.AuthUserId == authUserId && au.AccountId == accountId).SingleOrDefaultAsync();
            return accountUser != null;
        }

        public AccountUserRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
    }
}