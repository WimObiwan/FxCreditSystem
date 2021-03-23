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
            return await dataContext.UserIdentities.AnyAsync(ui => ui.User.ExternalId == userId && ui.Identity == identity);
        }

        internal async Task<bool> HasAccount(Guid userId, long accountId)
        {
            return await dataContext.AccountUsers.AnyAsync(au => au.User.ExternalId == userId && au.AccountId == accountId);
        }

        public async Task<IList<Common.Entities.AccountUser>> GetAccounts(Guid userId)
        {
            var set = dataContext.AccountUsers.Where(au => au.User.ExternalId == userId);
            return await mapper.ProjectTo<Common.Entities.AccountUser>(set).ToListAsync();
        }

        public UserRepository(DataContext dataContext, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }
    }
}