using System;
using Bogus;

namespace FxCreditSystem.Common.Fakers
{
    public class AccountFaker : Faker<Common.Entities.Account>
    {
        public AccountFaker()
        {
            StrictMode(true);
            RuleFor(a => a.Id, f => f.Random.Guid());
            RuleFor(a => a.Description, f => f.Lorem.Sentence(3, 8));
            RuleFor(a => a.MinimumCredits, f => Math.Round(f.Random.Decimal(-1m, -20m), 9));
            RuleFor(a => a.Credits, f => Math.Round(f.Random.Decimal(100m, 150m), 9));
            RuleFor(a => a.LastChangeUtc, f => f.Date.Recent(30));
        }
    }
}
