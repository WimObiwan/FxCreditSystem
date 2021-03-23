using Bogus;

namespace FxCreditSystem.Repository.Fakers
{
    public class UserFaker : Faker<Repository.Entities.User>
    {
        public UserFaker()
        {
            StrictMode(true);
            Ignore(u => u.Id);
            RuleFor(u => u.UserId, f => f.Random.Guid());
            RuleFor(u => u.Description, f => $"{f.Person.FirstName} {f.Person.LastName}");
            Ignore(u => u.AccountUsers);
            Ignore(u => u.Accounts);
        }
    }
}
