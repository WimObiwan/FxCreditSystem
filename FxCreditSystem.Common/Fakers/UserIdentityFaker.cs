using Bogus;

namespace FxCreditSystem.Common.Fakers
{
    public class UserIdentityFaker : Faker<Common.Entities.UserIdentity>
    {
        public UserIdentityFaker()
        {
            StrictMode(true);
            RuleFor(au => au.UserId, f => f.Random.Guid());
            RuleFor(au => au.Identity, f => f.Random.Identity());
        }
    }
}
