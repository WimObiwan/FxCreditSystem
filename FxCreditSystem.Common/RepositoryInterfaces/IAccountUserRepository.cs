using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FxCreditSystem.Common.Entities;

namespace FxCreditSystem.Common
{
    public interface IUserRepository
    {
        Task<bool> HasIdentity(Guid userId, string identity);
        Task<IList<AccountUser>> GetAccounts(Guid userId);
    }
}
