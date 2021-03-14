using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using FxCreditSystem.Common;
using FxCreditSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext dataContext;
        private readonly AccountUserRepository accountUserRepository;
        private readonly IMapper mapper;

        public async Task Add(string authUserId, Guid accountId, Guid transactionId, DateTime dateTimeUtc, string description, decimal creditsChange, Guid otherAccountId)
        {
            try
            {
                if (string.IsNullOrEmpty(authUserId))
                    throw new ArgumentException("Should not be null or empty", nameof(authUserId));
                if (creditsChange == 0m)
                    throw new ArgumentException("Should not be 0", nameof(creditsChange)); 

                using (var ts = await dataContext.Database.BeginTransactionAsync())
                {
                    var account = await dataContext.Accounts.SingleOrDefaultAsync(a => a.ExternalId == accountId)
                        ?? throw new AccountNotFoundException(accountId);

                    bool valid = await accountUserRepository.Get(account.Id, authUserId);
                    if (!valid)
                        throw new AccountNotFoundException(account.ExternalId);

                    if (creditsChange < 0m)
                        VerifyAccountMinimumCredits(account, creditsChange);

                    var otherAccount = await dataContext.Accounts.SingleOrDefaultAsync(a => a.ExternalId == otherAccountId)
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
                    
                    dataContext.Transactions.AddRange(transaction, otherTransaction);

                    await ts.CommitAsync();

                    await dataContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException x)
            {
                throw new DatabaseException(x);
            } 
        }

        public async Task<ICollection<Common.Entities.Transaction>> Get(string authUserId, Guid accountId, int limit = int.MaxValue, int offset = 0)
        {
            if (string.IsNullOrEmpty(authUserId))
                throw new ArgumentException("Should not be null or empty", nameof(authUserId));
            if (limit < 0)
                throw new ArgumentException("Should be zero or positive", nameof(limit));
            if (offset < 0)
                throw new ArgumentException("Should be zero or positive", nameof(offset));
            var account = await dataContext.Accounts.SingleOrDefaultAsync(a => a.ExternalId == accountId)
                ?? throw new AccountNotFoundException(accountId);
            bool valid = await accountUserRepository.Get(account.Id, authUserId);
            if (!valid)
                throw new AccountNotFoundException(account.ExternalId);

            var set = dataContext.Transactions.Where(t => t.Account.Id == account.Id);
            if (offset > 0)
                set = set.Skip(offset);
            if (limit < int.MaxValue)
                set = set.Take(limit);

            return await mapper.ProjectTo<Common.Entities.Transaction>(set).ToListAsync();
        }

        private void VerifyAccountMinimumCredits(Account account, decimal creditsChange)
        {
            decimal creditsNew = account.Credits + creditsChange;
            if (creditsNew < account.MinimumCredits)
                throw new AccountCreditsInsufficientException(account, creditsNew);
        }

        public TransactionRepository(DataContext dataContext, AccountUserRepository accountUserRepository, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.accountUserRepository = accountUserRepository;
            this.mapper = mapper;
        }
    }
}