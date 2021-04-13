using System.Threading.Tasks;
using FxCreditSystem.Common;
using FxCreditSystem.Common.Entities;
using FxCreditSystem.Common.Fakers;
using Moq;
using Xunit;

namespace FxCreditSystem.Core.Tests
{
    public class AuthorizationServiceTest
    {
        private async Task CheckAuthorizedUser(bool hasIdentity, bool admin)
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();

            var mockUserIdentityRepository = new Mock<IUserRepository>();
            mockUserIdentityRepository.Setup(u => u.CheckIdentityScope(identity, master.UserId)).ReturnsAsync(hasIdentity);
            mockUserIdentityRepository.Setup(u => u.CheckAdminScope(identity, AccessType.Read)).ReturnsAsync(admin);

            var authorizationService = new AuthorizationService(mockUserIdentityRepository.Object, null);
            var result = await authorizationService.CheckAuthorizedUser(identity, master.UserId, AccessType.Read);

            Assert.Equal(hasIdentity | admin, result);
            
            mockUserIdentityRepository.Verify(ur => ur.CheckIdentityScope(identity, master.UserId));

            if (!hasIdentity)
                mockUserIdentityRepository.Verify(ur => ur.CheckAdminScope(identity, AccessType.Read));
            mockUserIdentityRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CheckAuthorizedUser_ReturnsTrue()
        {
            await CheckAuthorizedUser(true, false);
        }

        [Fact]
        public async Task CheckAuthorizedUser_WithoutAccessAsAdmin_ReturnsTrue()
        {
            await CheckAuthorizedUser(false, true);
        }

        [Fact]
        public async Task CheckAuthorizedUser_WithoutAccess_ReturnsFalse()
        {
            await CheckAuthorizedUser(false, false);
        }

        private async Task CheckAuthorizedAccount(bool hasIdentity, bool admin)
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(ur => ur.CheckAdminScope(identity, AccessType.Read)).ReturnsAsync(admin);

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(ar => ar.CheckIdentity(master.AccountId, identity)).ReturnsAsync(hasIdentity);

            var authorizationService = new AuthorizationService(mockUserRepository.Object, mockAccountRepository.Object);
            var result = await authorizationService.CheckAuthorizedAccount(identity, master.AccountId, AccessType.Read);

            Assert.Equal(hasIdentity || admin, result);

            if (!hasIdentity)
                mockUserRepository.Verify(ur => ur.CheckAdminScope(identity, AccessType.Read));
            mockUserRepository.VerifyNoOtherCalls();
            
            mockAccountRepository.Verify(ar => ar.CheckIdentity(master.AccountId, identity));
            mockAccountRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CheckAuthorizedAccount_ShouldReturnTrue()
        {
            await CheckAuthorizedAccount(true, false);
        }

        [Fact]
        public async Task CheckAuthorizedAccount_WithoutAccess_ShouldReturnFalse()
        {
            await CheckAuthorizedAccount(false, false);
        }

        [Fact]
        public async Task CheckAuthorizedAccount_WithoutAccessAsAdmin_ShouldReturnTrue()
        {
            await CheckAuthorizedAccount(false, true);
        }
    }
}
