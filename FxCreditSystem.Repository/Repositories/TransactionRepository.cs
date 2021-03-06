using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FxCreditSystem.Common;
using FxCreditSystem.Common.Commands;
using FxCreditSystem.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext _dataContext;
        private readonly UserRepository _userRepository;
        private readonly IMapper _mapper;

        public async Task Add(AddTransactionCommand addTransactionCommand)
        {
            await Add(
                addTransactionCommand.UserId,
                addTransactionCommand.AccountId,
                addTransactionCommand.TransactionId,
                addTransactionCommand.DateTimeUtc,
                addTransactionCommand.Description,
                addTransactionCommand.CreditsChange,
                addTransactionCommand.OtherAccountId);
        }

        private async Task Add(Guid userId, Guid accountId, 
            Guid transactionId, DateTime dateTimeUtc, string description, decimal creditsChange, Guid otherAccountId)
        {
            if (creditsChange == 0m)
                throw new ArgumentException("Should not be 0", nameof(creditsChange));

            try
            {
                using (var ts = await _dataContext.Database.BeginTransactionAsync())
                {
                    var account = await _dataContext.Accounts.SingleOrDefaultAsync(a => a.ExternalId == accountId)
                        ?? throw new AccountNotFoundException(accountId);

                    bool valid = await _userRepository.HasAccount(userId, account.Id);
                    if (!valid)
                        throw new AccountNotFoundException(account.ExternalId);
                    
                    if (accountId.Equals(otherAccountId))
                        throw new TransactionBetweenSameAccountsException(account);

                    if (creditsChange < 0m)
                        VerifyAccountMinimumCredits(account, creditsChange);

                    var otherAccount = await _dataContext.Accounts.SingleOrDefaultAsync(a => a.ExternalId == otherAccountId)
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
                    
                    _dataContext.Transactions.AddRange(transaction, otherTransaction);

                    await ts.CommitAsync();

                    await _dataContext.SaveChangesAsync();
                }
            }
            catch (DbUpdateException x)
            {
                throw new DatabaseException(x);
            } 
        }

        public async Task<ICollection<Common.Entities.Transaction>> Get(Guid userId, Guid accountId, int limit = int.MaxValue, int offset = 0)
        {
            if (limit < 0)
                throw new ArgumentException("Should be zero or positive", nameof(limit));
            if (offset < 0)
                throw new ArgumentException("Should be zero or positive", nameof(offset));
            var account = await _dataContext.Accounts.SingleOrDefaultAsync(a => a.ExternalId == accountId)
                ?? throw new AccountNotFoundException(accountId);
            bool valid = await _userRepository.HasAccount(userId, account.Id);
            if (!valid)
                throw new AccountNotFoundException(account.ExternalId);

            var set = _dataContext.Transactions.Where(t => t.Account.Id == account.Id);
            if (offset > 0)
                set = set.Skip(offset);
            if (limit < int.MaxValue)
                set = set.Take(limit);

            return await _mapper.ProjectTo<Common.Entities.Transaction>(set).ToListAsync();
        }

        private void VerifyAccountMinimumCredits(Account account, decimal creditsChange)
        {
            decimal creditsNew = account.Credits + creditsChange;
            if (creditsNew < account.MinimumCredits)
                throw new AccountCreditsInsufficientException(account, creditsNew);
        }

        public TransactionRepository(DataContext dataContext, UserRepository userRepository, IMapper mapper)
        {
            _dataContext = dataContext;
            _userRepository = userRepository;
            _mapper = mapper;
        }
    }
}