using System;
using Bogus;
using FxCreditSystem.Common.Fakers;

namespace FxCreditSystem.Repository.Fakers
{
    public class TransactionFaker : Faker<Repository.Entities.Transaction>
    {
        public TransactionFaker()
        {
            StrictMode(true);
            Ignore(t => t.Id);
            RuleFor(t => t.ExternalId, f => f.Random.Guid());
            RuleFor(a => a.DateTimeUtc, f => f.Date.Recent(30));
            RuleFor(a => a.CreditsChange, f => f.Random.Money(-100m, 100m));
            RuleFor(a => a.CreditsNew, f => f.Random.Money(100m, 150m));
            RuleFor(a => a.Description, f => f.Lorem.Sentence(3, 8));
            Ignore(a => a.AccountId);
            Ignore(a => a.Account);
            Ignore(a => a.PrimaryTransactionId);
            Ignore(a => a.PrimaryTransaction);
        }
    }
}
