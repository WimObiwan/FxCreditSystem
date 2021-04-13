
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository.Tests.Shared
{
    public class DatabaseSeeder
    {
        private readonly DataContext dbContext;

        public Entities.User User { get; private set; }
        public Entities.User OtherUser { get; private set; }
        public Entities.User AdminUser { get; private set; }
        public Entities.Account Account { get; private set; }
        public Entities.Account OtherAccount { get; private set; }

        public void Seed()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var userFaker = new Fakers.UserFaker(); 
            User = userFaker.Generate();
            OtherUser = userFaker.Generate();
            AdminUser = userFaker.Generate();
            AdminUser.Scopes = new List<Entities.UserScope> { 
                new Entities.UserScope() {
                    Scope = "admin:read" 
                } 
            };
            dbContext.Users.AddRange(
                User,
                OtherUser,
                AdminUser
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
                    User = User,
                },
                new Entities.AccountUserLink
                {
                    Account = OtherAccount,
                    User = OtherUser,
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