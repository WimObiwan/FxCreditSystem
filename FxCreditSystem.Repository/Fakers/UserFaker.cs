using System.Collections.Generic;
using Bogus;

namespace FxCreditSystem.Repository.Fakers
{
    public class UserFaker : Faker<Repository.Entities.User>
    {
        public UserFaker()
        {
            var userIdentityFaker = new UserIdentityFaker();
            StrictMode(true);
            Ignore(u => u.Id);
            RuleFor(u => u.ExternalId, f => f.Random.Guid());
            RuleFor(u => u.Description, f => $"{f.Person.FirstName} {f.Person.LastName}");
            Ignore(u => u.AccountUsers);
            Ignore(u => u.Accounts);
            RuleFor(u => u.Identities, f => userIdentityFaker.Generate(3));
            RuleFor(u => u.Scopes, f => new List<Repository.Entities.UserScope>());
        }
    }
}
