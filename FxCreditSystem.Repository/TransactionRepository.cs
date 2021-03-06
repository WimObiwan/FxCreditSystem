using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using FxCreditSystem.Common;
using FxCreditSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext dataContext;

        public async Task Add(Guid accountId, Guid transactionId, DateTime dateTimeUtc, string description, decimal creditsChange, Guid otherAccountId)
        {
            if (creditsChange == 0m)
                throw new ArgumentException("Should not be 0", nameof(creditsChange)); 

            using (var ts = await dataContext.Database.BeginTransactionAsync())
            {
                var account = await dataContext.Accounts.FirstOrDefaultAsync(a => a.ExternalId == accountId)
                    ?? throw new AccountNotFoundException(accountId);
                if (creditsChange < 0m)
                {
                    VerifyAccountMinimumCredits(account, creditsChange);
                }


                var otherAccount = await dataContext.Accounts.FirstOrDefaultAsync(a => a.ExternalId == otherAccountId)
                    ?? throw new AccountNotFoundException(accountId);
                if (creditsChange > 0m)
                {
                    throw new DebetFromOtherAccountNotAllowedException(account, otherAccount);
                    //VerifyAccountMinimumCredits(otherAccount, -creditsChange);
                }

                account.Credits += creditsChange;
                otherAccount.Credits -= creditsChange;

                account.LastChangeUtc = dateTimeUtc;
                otherAccount.LastChangeUtc = dateTimeUtc;

                Entities.Transaction transaction = new Entities.Transaction()
                {
                    Account = account,
                    CreditsChange = creditsChange,
                    CreditsNew = account.Credits,
                    Description = description,
                    ExternalId = transactionId,
                    DateTimeUtc = dateTimeUtc
                };

                Entities.Transaction otherTransaction = new Entities.Transaction()
                {
                    Account = otherAccount,
                    CreditsChange = -creditsChange,
                    CreditsNew = otherAccount.Credits,
                    Description = description,
                    ExternalId = transactionId,
                    DateTimeUtc = dateTimeUtc
                };

                otherTransaction.PrimaryTransaction = transaction;
                
                dataContext.AccountHistory.AddRange(transaction, otherTransaction);

                await ts.CommitAsync();

                await dataContext.SaveChangesAsync();
            }
        }

        private void VerifyAccountMinimumCredits(Account account, decimal creditsChange)
        {
            decimal creditsNew = account.Credits + creditsChange;
            if (creditsNew < account.MinimumCredits)
                throw new AccountCreditsInsufficientException(account, creditsNew);
        }

        public TransactionRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
    }
}