using System;
using System.Threading.Tasks;
using FxCreditSystem.Common;

namespace FxCreditSystem.Logic
{
    public class Transaction
    {
        private readonly ITransactionRepository transactionRepository;
        
        public async Task Add(FxCreditSystem.Common.Entities.TransactionAdd transactionAdd)
        {
            await transactionRepository.Add(transactionAdd);
        }

        public Transaction(ITransactionRepository transactionRepository) 
        {
            this.transactionRepository = transactionRepository;
        }
    }
}
