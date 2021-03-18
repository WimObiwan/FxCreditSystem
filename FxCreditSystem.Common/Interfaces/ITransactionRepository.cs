using System.Threading.Tasks;
using FxCreditSystem.Common.Commands;

namespace FxCreditSystem.Common
{
    public interface ITransactionRepository
    {
        Task Add(AddTransactionCommand transactionAdd);
    }
}
