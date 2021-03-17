using System.Threading.Tasks;
using DeepEqual.Syntax;
using FxCreditSystem.Common;
using Moq;
using Xunit;

namespace FxCreditSystem.Logic.Test
{
    public class UserTest
    {

        [Fact]
        public async Task GetAccountsForUser_ShouldSucceed()
        {
            var accountUserFaker = new Common.Fakers.AccountUserFaker();
            var master = accountUserFaker.Generate();
            var newAccountUserFaker = accountUserFaker
                .RuleFor(au => au.AuthUserId, master.AuthUserId)
                .RuleFor(au => au.UserDescription, master.AccountDescription);

            var originalAccountUserList = newAccountUserFaker.Generate(3);

            var mockAccountUserRepository = new Mock<IAccountUserRepository>();
            mockAccountUserRepository.Setup(au => au.Get(master.AuthUserId)).ReturnsAsync(originalAccountUserList);

            var user = new User(mockAccountUserRepository.Object);
            var accountUserList = await user.GetAccountsForUser(master.AuthUserId);

            originalAccountUserList.ShouldDeepEqual(accountUserList);

            mockAccountUserRepository.Verify(au => au.Get(master.AuthUserId));
            mockAccountUserRepository.VerifyNoOtherCalls();
        }
    }
}
