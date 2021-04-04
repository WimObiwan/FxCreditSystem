using System;
using System.Threading.Tasks;
using FxCreditSystem.Common.Entities;

namespace FxCreditSystem.Common
{
    public interface IAccountRepository
    {
        Task<bool> HasIdentity(Guid accountId, string identity);
        Task<Account> GetAccount(Guid accountId);
    }
}
