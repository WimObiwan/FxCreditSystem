using System;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using FxCreditSystem.Common;
using FxCreditSystem.Common.Fakers;
using Moq;
using Xunit;

namespace FxCreditSystem.Core.Tests
{
    public class AccountQueryHandlerTest
    {

        [Fact]
        public async Task GetAccount_WithoutAccess_ShouldFail()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountUserFaker = new Common.Fakers.AccountFaker();
            var originalAccount = accountUserFaker.Generate();

            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(a => a.CheckAuthorizedUser(identity, originalAccount.Id, AccessType.Read)).ReturnsAsync(false);

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(ar => ar.GetAccount(originalAccount.Id)).ReturnsAsync(originalAccount);

            var accountQueryHandler = new AccountQueryHandler(mockAuthorizationService.Object, mockAccountRepository.Object);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await accountQueryHandler.GetAccount(identity, originalAccount.Id));

            mockAuthorizationService.Verify(a => a.CheckAuthorizedAccount(identity, originalAccount.Id, AccessType.Read));
            mockAuthorizationService.VerifyNoOtherCalls();

            mockAccountRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetAccount_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountUserFaker = new Common.Fakers.AccountFaker();
            var originalAccount = accountUserFaker.Generate();

            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(a => a.CheckAuthorizedAccount(identity, originalAccount.Id, AccessType.Read)).ReturnsAsync(true);

            var mockAccountRepository = new Mock<IAccountRepository>();
            mockAccountRepository.Setup(ar => ar.GetAccount(originalAccount.Id)).ReturnsAsync(originalAccount);

            var accountQueryHandler = new AccountQueryHandler(mockAuthorizationService.Object, mockAccountRepository.Object);
            var account = await accountQueryHandler.GetAccount(identity, originalAccount.Id);

            originalAccount.ShouldDeepEqual(account);

            mockAuthorizationService.Verify(a => a.CheckAuthorizedAccount(identity, originalAccount.Id, AccessType.Read));
            mockAuthorizationService.VerifyNoOtherCalls();

            mockAccountRepository.Verify(ar => ar.GetAccount(originalAccount.Id));
            mockAccountRepository.VerifyNoOtherCalls();
        }
    }
}
