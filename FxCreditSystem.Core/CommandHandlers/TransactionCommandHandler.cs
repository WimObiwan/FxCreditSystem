using System.Threading.Tasks;
using FxCreditSystem.Common;
using FxCreditSystem.Common.Commands;

namespace FxCreditSystem.Core
{
    public class TransactionCommandHandler : ITransactionCommandHandler
    {
        private readonly ITransactionRepository transactionRepository;

        public async Task HandleAsync(AddTransactionCommand addTransactionCommand)
        {
            await transactionRepository.Add(addTransactionCommand);
        }

        public TransactionCommandHandler(ITransactionRepository transactionRepository) 
        {
            this.transactionRepository = transactionRepository;
        }
    }
}
