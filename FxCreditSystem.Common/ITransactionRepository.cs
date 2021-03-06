using System;
using System.Threading.Tasks;

namespace FxCreditSystem.Common
{
    public interface ITransactionRepository
    {
        Task Add(Guid accountId, Guid transactionId, DateTime dateTimeUtc, string description, decimal creditsChange, Guid otherAccountId);
    }
}
