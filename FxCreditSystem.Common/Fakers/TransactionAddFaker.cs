using System;
using Bogus;
using FxCreditSystem.Common.Commands;

namespace FxCreditSystem.Common.Fakers
{
    public class AddTransactionCommandFaker : Faker<AddTransactionCommand>
    {
        public AddTransactionCommandFaker()
        {
            StrictMode(true);
            RuleFor(ta => ta.UserId, f => f.Random.Guid());
            RuleFor(ta => ta.AccountId, f => f.Random.Guid());
            RuleFor(ta => ta.TransactionId, f => f.Random.Guid());
            RuleFor(ta => ta.DateTimeUtc, f => f.Date.Recent(30));
            RuleFor(ta => ta.Description, f => f.Lorem.Sentence(3, 8));
            RuleFor(ta => ta.CreditsChange, f => Math.Round(f.Random.Decimal(-1m, -50m), 9));
            RuleFor(ta => ta.OtherAccountId, f => f.Random.Guid());
        }
    }
}
