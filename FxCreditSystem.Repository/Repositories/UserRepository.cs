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
    public class UserRepository : IUserRepository
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        public async Task<bool> HasIdentity(Guid userId, string identity)
        {
            // We need to know difference between identity not found ("false") & user not found (UserNotFoundException)
            var result = await dataContext.Users
                .Where(u => u.ExternalId == userId)
                .Select(u => (bool?)u.Identities.Any(ui => ui.Identity == identity))
                .FirstOrDefaultAsync<bool?>();
            return result ?? throw new UserNotFoundException(userId);
        }

        internal async Task<bool> HasAccount(Guid userId, long accountId)
        {
            return await dataContext.AccountUsers.AnyAsync(au => au.User.ExternalId == userId && au.AccountId == accountId);
        }

        public async Task<IList<Common.Entities.UserIdentity>> GetIdentities(Guid userId)
        {
            // We need to know difference between identity not found (empty list) & user not found (UserNotFoundException)
            var result = await dataContext.Users
                .Where(u => u.ExternalId == userId)
                .Include(u => u.Identities)
                .SingleOrDefaultAsync()
                ?? throw new UserNotFoundException(userId);

            return mapper.Map<List<Common.Entities.UserIdentity>>(result.Identities);
        }

        public async Task<IList<Common.Entities.AccountUser>> GetAccounts(Guid userId)
        {
            // We need to know difference between identity not found (empty list) & user not found (UserNotFoundException)
            var result = await dataContext.Users
                .Where(u => u.ExternalId == userId)
                .Include(u => u.Accounts)
                .SingleOrDefaultAsync()
                ?? throw new UserNotFoundException(userId);

            return mapper.Map<List<Common.Entities.AccountUser>>(result.AccountUsers);
        }

        public UserRepository(DataContext dataContext, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }
    }
}