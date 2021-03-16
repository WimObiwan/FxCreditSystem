using Bogus;

namespace FxCreditSystem.Common.Fakers
{
    public class TransactionAddFaker : Faker<Common.Entities.TransactionAdd>
    {
        public TransactionAddFaker()
        {
            StrictMode(true);
            RuleFor(ta => ta.AuthUserId, f => f.Random.String(30));
            RuleFor(ta => ta.AccountId, f => f.Random.Guid());
            RuleFor(ta => ta.TransactionId, f => f.Random.Guid());
            RuleFor(ta => ta.DateTimeUtc, f => f.Date.Recent(30));
            RuleFor(ta => ta.Description, f => f.Lorem.Sentence(3, 8));
            RuleFor(ta => ta.CreditsChange, f => f.Random.Decimal(50m, 150m));
            RuleFor(ta => ta.OtherAccountId, f => f.Random.Guid());
        }
    }
}
