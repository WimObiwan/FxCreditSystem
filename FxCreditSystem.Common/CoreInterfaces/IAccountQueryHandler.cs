using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FxCreditSystem.Common
{
    public interface IAccountQueryHandler
    {
        Task<Common.Entities.Account> GetAccount(string identity, Guid accountId);
    }
}
