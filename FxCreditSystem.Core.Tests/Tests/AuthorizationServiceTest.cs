using System.Threading.Tasks;
using FxCreditSystem.Common;
using FxCreditSystem.Common.Fakers;
using Moq;
using Xunit;

namespace FxCreditSystem.Core.Tests
{
    public class AuthorizationServiceTest
    {
        private async Task CheckAuthorizedUser(bool hasIdentity)
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();

            var mockUserIdentityRepository = new Mock<IUserRepository>();
            mockUserIdentityRepository.Setup(u => u.CheckIdentityScope(identity, master.UserId)).ReturnsAsync(hasIdentity);

            var authorizationService = new AuthorizationService(mockUserIdentityRepository.Object, null);
            var result = await authorizationService.CheckAuthorizedUser(identity, master.UserId, AccessType.Read);

            Assert.Equal(hasIdentity, result);
            
            mockUserIdentityRepository.Verify(u => u.HasIdentity(master.UserId, identity));
            mockUserIdentityRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CheckAuthorizedUser_ShouldSucceed()
        {
            await CheckAuthorizedUser(true);
        }

        [Fact]
        public async Task CheckAuthorizedUser_WithoutAccess_ShouldSucceed()
        {
            await CheckAuthorizedUser(false);
        }

        private async Task CheckAuthorizedAccount(bool hasIdentity)
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(ar => ar.CheckIdentity(master.AccountId, identity)).ReturnsAsync(hasIdentity);

            var authorizationService = new AuthorizationService(null, mockAccountRepository.Object);
            var result = await authorizationService.CheckAuthorizedAccount(identity, master.AccountId, AccessType.Read);

            Assert.Equal(hasIdentity, result);
            
            mockAccountRepository.Verify(a => a.HasIdentity(master.AccountId, identity));
            mockAccountRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CheckAuthorizedAccount_ShouldSucceed()
        {
            await CheckAuthorizedAccount(true);
        }

        [Fact]
        public async Task CheckAuthorizedAccount_WithoutAccess_ShouldSucceed()
        {
            await CheckAuthorizedAccount(false);
        }
    }
}
