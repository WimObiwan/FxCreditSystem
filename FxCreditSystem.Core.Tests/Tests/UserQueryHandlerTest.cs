using System;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using FxCreditSystem.Common;
using FxCreditSystem.Common.Fakers;
using Moq;
using Xunit;

namespace FxCreditSystem.Core.Tests
{
    public class UserQueryHandlerTest
    {

        [Fact]
        public async Task GetIdentities_WithoutAccess_ShouldFail()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();
            var newAccountUserFaker = accountUserFaker
                .RuleFor(au => au.UserId, master.UserId)
                .RuleFor(au => au.UserDescription, master.AccountDescription);

            var originalAccountUserList = newAccountUserFaker.Generate(3);

            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(a => a.CheckAuthorizedUser(identity, master.UserId)).ReturnsAsync(false);

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(ur => ur.GetAccounts(master.UserId)).ReturnsAsync(originalAccountUserList);

            var userQueryHandler = new UserQueryHandler(mockAuthorizationService.Object, mockUserRepository.Object);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await userQueryHandler.GetIdentities(identity, master.UserId));

            mockUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetIdentities_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var userIdentityFaker = new Common.Fakers.UserIdentityFaker();
            var master = userIdentityFaker.Generate();
            var newUserIdentityFaker = userIdentityFaker
                .RuleFor(au => au.UserId, master.UserId);

            var originalUserIdentityList = newUserIdentityFaker.Generate(3);

            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(a => a.CheckAuthorizedUser(identity, master.UserId)).ReturnsAsync(true);

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(ur => ur.GetIdentities(master.UserId)).ReturnsAsync(originalUserIdentityList);

            var userQueryHandler = new UserQueryHandler(mockAuthorizationService.Object, mockUserRepository.Object);
            var userIdentitiesList = await userQueryHandler.GetIdentities(identity, master.UserId);

            originalUserIdentityList.ShouldDeepEqual(userIdentitiesList);

            mockAuthorizationService.Verify(a => a.CheckAuthorizedUser(identity, master.UserId));
            mockAuthorizationService.VerifyNoOtherCalls();

            mockUserRepository.Verify(ur => ur.GetIdentities(master.UserId));
            mockUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAccounts_WithoutAccess_ShouldFail()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();
            var newAccountUserFaker = accountUserFaker
                .RuleFor(au => au.UserId, master.UserId)
                .RuleFor(au => au.UserDescription, master.AccountDescription);

            var originalAccountUserList = newAccountUserFaker.Generate(3);

            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(a => a.CheckAuthorizedUser(identity, master.UserId)).ReturnsAsync(false);

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(ur => ur.GetAccounts(master.UserId)).ReturnsAsync(originalAccountUserList);

            var userQueryHandler = new UserQueryHandler(mockAuthorizationService.Object, mockUserRepository.Object);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await userQueryHandler.GetAccounts(identity, master.UserId));

            mockUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAccounts_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();
            var newAccountUserFaker = accountUserFaker
                .RuleFor(au => au.UserId, master.UserId)
                .RuleFor(au => au.UserDescription, master.AccountDescription);

            var originalAccountUserList = newAccountUserFaker.Generate(3);

            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(a => a.CheckAuthorizedUser(identity, master.UserId)).ReturnsAsync(true);

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(ur => ur.GetAccounts(master.UserId)).ReturnsAsync(originalAccountUserList);

            var userQueryHandler = new UserQueryHandler(mockAuthorizationService.Object, mockUserRepository.Object);
            var accountUserList = await userQueryHandler.GetAccounts(identity, master.UserId);

            originalAccountUserList.ShouldDeepEqual(accountUserList);

            mockAuthorizationService.Verify(a => a.CheckAuthorizedUser(identity, master.UserId));
            mockAuthorizationService.VerifyNoOtherCalls();

            mockUserRepository.Verify(ur => ur.GetAccounts(master.UserId));
            mockUserRepository.VerifyNoOtherCalls();
        }
    }
}
