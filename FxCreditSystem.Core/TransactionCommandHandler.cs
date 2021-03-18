using System.Threading.Tasks;
using FxCreditSystem.Common;

namespace FxCreditSystem.Core
{
    public class TransactionCommandHandler
    {
        private readonly ITransactionRepository transactionRepository;
        
        public async Task HandleAsync(FxCreditSystem.Common.Entities.AddTransactionCommand addTransactionCommand)
        {
            await transactionRepository.Add(addTransactionCommand);
        }

        public TransactionCommandHandler(ITransactionRepository transactionRepository) 
        {
            this.transactionRepository = transactionRepository;
        }
    }
}
