using Bogus;

namespace FxCreditSystem.Common.Fakers
{
    public class AccountUserFaker : Faker<Common.Entities.AccountUser>
    {
        public AccountUserFaker()
        {
            StrictMode(true);
            RuleFor(au => au.AuthUserId, f => $"test|{f.Random.Hexadecimal(16, "")}");
            RuleFor(au => au.UserDescription, f => $"{f.Person.FirstName} {f.Person.LastName}");
            RuleFor(au => au.AccountId, f => f.Random.Guid());
            RuleFor(au => au.AccountDescription, f => f.Lorem.Sentence(3, 8));
        }
    }
}
