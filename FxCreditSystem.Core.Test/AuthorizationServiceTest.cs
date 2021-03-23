using System;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using FxCreditSystem.Common;
using Moq;
using Xunit;

namespace FxCreditSystem.Core.Test
{
    public class AuthorizationServiceTest
    {
        [Fact]
        public async Task CheckAuthorizedUser_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = $"test|{faker.Random.Hexadecimal(16, "")}";

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();

            var mockUserIdentityRepository = new Mock<IUserRepository>();
            mockUserIdentityRepository.Setup(u => u.HasIdentity(master.UserId, identity)).ReturnsAsync(true);

            var authorizationService = new AuthorizationService(mockUserIdentityRepository.Object);
            var result = await authorizationService.CheckAuthorizedUser(identity, master.UserId);

            Assert.True(result);
            
            mockUserIdentityRepository.Verify(u => u.HasIdentity(master.UserId, identity));
            mockUserIdentityRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CheckAuthorizedUser_WithoutAccess_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = $"test|{faker.Random.Hexadecimal(16, "")}";

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();

            var mockUserIdentityRepository = new Mock<IUserRepository>();
            mockUserIdentityRepository.Setup(u => u.HasIdentity(master.UserId, identity)).ReturnsAsync(false);

            var authorizationService = new AuthorizationService(mockUserIdentityRepository.Object);
            var result = await authorizationService.CheckAuthorizedUser(identity, master.UserId);

            Assert.False(result);
            
            mockUserIdentityRepository.Verify(u => u.HasIdentity(master.UserId, identity));
            mockUserIdentityRepository.VerifyNoOtherCalls();
        }
    }
}
