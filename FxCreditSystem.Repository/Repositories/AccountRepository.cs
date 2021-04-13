using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using FxCreditSystem.Common;
using FxCreditSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        public AccountRepository(DataContext dataContext, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }

        public async Task<bool> CheckIdentity(Guid accountId, string identity)
        {
            // We need to know difference between identity not found ("false") & user not found (UserNotFoundException)
            var result = await dataContext.Accounts
                .Where(a => a.ExternalId == accountId)
                .SelectMany(a => a.Users)
                .Select(u => (bool?)u.Identities.Any(ui => ui.Identity == identity))
                .FirstOrDefaultAsync<bool?>();
            return result ?? throw new AccountNotFoundException(accountId);
        }

        public async Task<Common.Entities.Account> GetAccount(Guid accountId)
        {
            // We need to know difference between identity not found (empty list) & user not found (UserNotFoundException)
            var result = await dataContext.Accounts
                .Where(u => u.ExternalId == accountId)
                .SingleOrDefaultAsync()
                ?? throw new AccountNotFoundException(accountId);

            return mapper.Map<Common.Entities.Account>(result);
        }
   }
}