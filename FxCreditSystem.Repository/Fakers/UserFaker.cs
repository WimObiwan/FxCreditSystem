using Bogus;

namespace FxCreditSystem.Repository.Fakers
{
    public class UserFaker : Faker<Repository.Entities.User>
    {
        public UserFaker()
        {
            StrictMode(true);
            Ignore(u => u.Id);
            RuleFor(u => u.AuthUserId, f => $"test|{f.Random.Hexadecimal(16, "")}");
            RuleFor(u => u.Description, f => $"{f.Person.FirstName} {f.Person.LastName}");
            Ignore(u => u.AccountUsers);
            Ignore(u => u.Accounts);
        }
    }
}
