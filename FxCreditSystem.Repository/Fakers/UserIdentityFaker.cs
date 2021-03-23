using Bogus;

namespace FxCreditSystem.Repository.Fakers
{
    public class UserIdentityFaker : Faker<Repository.Entities.UserIdentity>
    {
        public UserIdentityFaker()
        {
            StrictMode(true);
            Ignore(ui => ui.Id);
            RuleFor(ui => ui.Identity, f => $"test|{f.Random.Hexadecimal(16, "")}");
            Ignore(ui => ui.UserId);
            Ignore(ui => ui.User);
        }
    }
}
