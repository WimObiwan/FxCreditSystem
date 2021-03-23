using Bogus;
using FxCreditSystem.Common.Fakers;

namespace FxCreditSystem.Repository.Fakers
{
    public class UserIdentityFaker : Faker<Repository.Entities.UserIdentity>
    {
        public UserIdentityFaker()
        {
            StrictMode(true);
            Ignore(ui => ui.Id);
            RuleFor(ui => ui.Identity, f => f.Random.Identity());
            Ignore(ui => ui.UserId);
            Ignore(ui => ui.User);
        }
    }
}
