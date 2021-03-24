
using System;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository.Test.Shared
{
    public class DatabaseSeeder
    {
        private readonly DataContext dbContext;

        public Entities.User User { get; private set; }
        public Entities.User OtherUser { get; private set; }
        public Entities.Account Account { get; private set; }
        public Entities.Account OtherAccount { get; private set; }

        public void Seed()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var userFaker = new Fakers.UserFaker(); 
            var user = userFaker.Generate();
            var otherUser = userFaker.Generate();
            User = user;
            OtherUser = otherUser;
            dbContext.Users.AddRange(
                user,
                otherUser
            );

            Fakers.AccountFaker accountFaker = new Fakers.AccountFaker();
            Account = accountFaker.Generate();
            OtherAccount = accountFaker.Generate();

            dbContext.Accounts.AddRange(
                Account,
                OtherAccount
            );

            dbContext.AccountUsers.AddRange(
                new Entities.AccountUserLink
                {
                    Account = Account,
                    User = user,
                },
                new Entities.AccountUserLink
                {
                    Account = OtherAccount,
                    User = otherUser,
                }
            );

            var transactionFaker = new Fakers.TransactionFaker();
            var transaction = transactionFaker
                .RuleFor(t => t.Account, Account)
                .RuleFor(t => t.CreditsChange, Account.Credits)
                .RuleFor(t => t.CreditsNew, Account.Credits)
                .Generate();
            var otherTransaction = transactionFaker
                .RuleFor(t => t.Account, OtherAccount)
                .RuleFor(t => t.CreditsChange, OtherAccount.Credits)
                .RuleFor(t => t.CreditsNew, OtherAccount.Credits)
                .RuleFor(t => t.PrimaryTransaction, transaction)
                .Generate();

            dbContext.AddRange(
                transaction,
                otherTransaction
            );

            dbContext.SaveChanges();

            dbContext.Entry(User).State = EntityState.Detached;
            dbContext.Entry(OtherUser).State = EntityState.Detached;
            dbContext.Entry(Account).State = EntityState.Detached;
            dbContext.Entry(OtherAccount).State = EntityState.Detached;
        }

        public DatabaseSeeder(DataContext dbContext)
        {
            this.dbContext = dbContext;
        }

   }
}