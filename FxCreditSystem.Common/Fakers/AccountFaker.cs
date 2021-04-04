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
            RuleFor(a => a.MinimumCredits, f => f.Random.Money(-1m, -20m));
            RuleFor(a => a.Credits, f => f.Random.Money(100m, 150m));
            RuleFor(a => a.LastChangeUtc, f => f.Date.Recent(30));
        }
    }
}
