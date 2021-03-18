using System;
using System.Threading.Tasks;

namespace FxCreditSystem.Common
{
    public interface ITransactionRepository
    {
        Task Add(Entities.AddTransactionCommand transactionAdd);
    }
}
