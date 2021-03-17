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
            RuleFor(a => a.MinimumCredits, f => f.Random.Decimal(-1m, -20m));
            RuleFor(a => a.Credits, f => f.Random.Decimal(100m, 150m));
            RuleFor(a => a.LastChangeUtc, f => f.Date.Recent(30));
            Ignore(a => a.Transactions);
            Ignore(a => a.AccountUsers);
            Ignore(a => a.Users);
        }
    }
}
