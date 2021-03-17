
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository.Test.Shared
{
    internal class DatabaseSeeder
    {
        private readonly DataContext dbContext;

        public string AuthUserId { get; private set; }
        public string OtherAuthUserId { get; private set; }
        public Entities.Account Account { get; private set; }
        public Entities.Account OtherAccount { get; private set; }

        public void Seed()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var userFaker = new Fakers.UserFaker(); 
            var user = userFaker.Generate();
            var otherUser = userFaker.Generate();
            AuthUserId = user.AuthUserId;
            OtherAuthUserId = otherUser.AuthUserId;
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
                new Entities.AccountUser
                {
                    Account = Account,
                    User = user,
                },
                new Entities.AccountUser
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

            dbContext.Entry(Account).State = EntityState.Detached;
            dbContext.Entry(OtherAccount).State = EntityState.Detached;
        }

        public DatabaseSeeder(DataContext dbContext)
        {
            this.dbContext = dbContext;
        }

   }
}