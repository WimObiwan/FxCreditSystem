using System;
using Bogus;

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
            RuleFor(a => a.CreditsChange, f => Math.Round(f.Random.Decimal(-100m, 100m), 9));
            RuleFor(a => a.CreditsNew, f => Math.Round(f.Random.Decimal(100m, 150m), 9));
            RuleFor(a => a.Description, f => f.Lorem.Sentence(3, 8));
            Ignore(a => a.AccountId);
            Ignore(a => a.Account);
            Ignore(a => a.PrimaryTransactionId);
            Ignore(a => a.PrimaryTransaction);
        }
    }
}
