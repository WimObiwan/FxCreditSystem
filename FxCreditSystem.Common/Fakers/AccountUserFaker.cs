using Bogus;

namespace FxCreditSystem.Common.Fakers
{
    public class AccountUserFaker : Faker<Common.Entities.AccountUser>
    {
        public AccountUserFaker()
        {
            StrictMode(true);
            RuleFor(au => au.AccountId, f => f.Random.Guid());
            RuleFor(au => au.AccountDescription, f => f.Lorem.Sentence(3, 8));
            RuleFor(au => au.UserId, f => f.Random.Guid());
            RuleFor(au => au.UserDescription, f => $"{f.Person.FirstName} {f.Person.LastName}");
        }
    }
}
