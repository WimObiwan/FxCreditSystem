using System;
using System.Threading.Tasks;
using FxCreditSystem.Common;

namespace FxCreditSystem.Logic
{
    public class Transaction
    {
        private readonly ITransactionRepository transactionRepository;
        
        public async Task Add(string authUserId, Guid accountId, Guid transactionId, DateTime dateTimeUtc, string description, decimal creditsChange, Guid otherAccountId)
        {
            await this.transactionRepository.Add(authUserId, accountId, transactionId, dateTimeUtc, description, creditsChange, otherAccountId);
        }

        public Transaction(ITransactionRepository transactionRepository) 
        {
            this.transactionRepository = transactionRepository;
        }
    }
}
