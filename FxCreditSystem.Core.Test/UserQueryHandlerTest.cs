using System;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using FxCreditSystem.Common;
using Moq;
using Xunit;

namespace FxCreditSystem.Core.Test
{
    public class UserQueryHandlerTest
    {

        [Fact]
        public async Task GetAccountsForUser_WithoutAccess_ShouldFail()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = $"test|{faker.Random.Hexadecimal(16, "")}";

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();
            var newAccountUserFaker = accountUserFaker
                .RuleFor(au => au.UserId, master.UserId)
                .RuleFor(au => au.UserDescription, master.AccountDescription);

            var originalAccountUserList = newAccountUserFaker.Generate(3);

            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(a => a.CheckAuthorizedUser(identity, master.UserId)).ReturnsAsync(false);

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(au => au.GetAccounts(master.UserId)).ReturnsAsync(originalAccountUserList);

            var userQueryHandler = new UserQueryHandler(mockAuthorizationService.Object, mockUserRepository.Object);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await userQueryHandler.GetAccounts(identity, master.UserId));

            mockUserRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAccountsForUser_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = $"test|{faker.Random.Hexadecimal(16, "")}";

            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();
            var newAccountUserFaker = accountUserFaker
                .RuleFor(au => au.UserId, master.UserId)
                .RuleFor(au => au.UserDescription, master.AccountDescription);

            var originalAccountUserList = newAccountUserFaker.Generate(3);

            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(a => a.CheckAuthorizedUser(identity, master.UserId)).ReturnsAsync(true);

            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(au => au.GetAccounts(master.UserId)).ReturnsAsync(originalAccountUserList);

            var userQueryHandler = new UserQueryHandler(mockAuthorizationService.Object, mockUserRepository.Object);
            var accountUserList = await userQueryHandler.GetAccounts(identity, master.UserId);

            originalAccountUserList.ShouldDeepEqual(accountUserList);

            mockAuthorizationService.Verify(a => a.CheckAuthorizedUser(identity, master.UserId));
            mockAuthorizationService.VerifyNoOtherCalls();

            mockUserRepository.Verify(au => au.GetAccounts(master.UserId));
            mockUserRepository.VerifyNoOtherCalls();
        }
    }
}
