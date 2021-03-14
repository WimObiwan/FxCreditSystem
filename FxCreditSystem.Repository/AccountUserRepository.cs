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
    public class AccountUserRepository
    {
        private readonly DataContext dataContext;
        private readonly IMapper mapper;

        internal async Task<bool> Get(long accountId, string authUserId)
        {
            var accountUser = await dataContext.AccountUsers.Where(au => au.User.AuthUserId == authUserId && au.AccountId == accountId).SingleOrDefaultAsync();
            return accountUser != null;
        }

        public async Task<List<Common.Entities.AccountUser>> Get(string authUserId)
        {
            if (string.IsNullOrEmpty(authUserId))
                throw new ArgumentException("Should not be null or empty", nameof(authUserId));

            var set = dataContext.AccountUsers.Where(au => au.User.AuthUserId == authUserId);
            return await mapper.ProjectTo<Common.Entities.AccountUser>(set).ToListAsync();
        }

        public AccountUserRepository(DataContext dataContext, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.mapper = mapper;
        }
    }
}