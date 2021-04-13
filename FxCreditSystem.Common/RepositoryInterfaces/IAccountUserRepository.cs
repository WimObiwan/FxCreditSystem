using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FxCreditSystem.Common.Entities;

namespace FxCreditSystem.Common
{
    public interface IUserRepository
    {
        Task<bool> CheckIdentityScope(string identity, Guid userId);
        Task<IList<UserIdentity>> GetIdentities(Guid userId);
        Task<IList<AccountUser>> GetAccounts(Guid userId);
    }
}
