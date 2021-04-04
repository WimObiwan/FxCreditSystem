using System;
using Bogus;

namespace FxCreditSystem.Repository.Fakers
{
    public class AccountFaker : Faker<Repository.Entities.Account>
    {
        public AccountFaker()
        {
            StrictMode(true);
            Ignore(a => a.Id);
            RuleFor(a => a.ExternalId, f => f.Random.Guid());
            RuleFor(a => a.Description, f => f.Lorem.Sentence(3, 8));
            RuleFor(a => a.MinimumCredits, f => Math.Round(f.Random.Decimal(-1m, -20m), 9));
            RuleFor(a => a.Credits, f => Math.Round(f.Random.Decimal(100m, 150m), 9));
            RuleFor(a => a.LastChangeUtc, f => f.Date.Recent(30));
            Ignore(a => a.Transactions);
            Ignore(a => a.AccountUsers);
            Ignore(a => a.Users);
        }
    }
}
