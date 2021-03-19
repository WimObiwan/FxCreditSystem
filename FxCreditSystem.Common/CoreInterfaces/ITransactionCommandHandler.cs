using System.Threading.Tasks;
using FxCreditSystem.Common.Commands;

namespace FxCreditSystem.Common
{
    public interface ITransactionCommandHandler
    {
        Task HandleAsync(AddTransactionCommand addTransactionCommand);
    }
}
