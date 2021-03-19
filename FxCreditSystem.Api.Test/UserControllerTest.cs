using FxCreditSystem.Api.Controllers;
using FxCreditSystem.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FxCreditSystem.Api.Test
{
    public class UserControllerTest
    {
        [Fact]
        public void GetAccounts_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            string authUserId = $"test|{faker.Random.Hexadecimal(16, "")}";

            var logger = new Mock<ILogger<UserController>>();
            var mockUserQueryHandler = new Mock<IUserQueryHandler>();
            var userController = new UserController(logger.Object, mockUserQueryHandler.Object);
            
            var account = userController.Get(authUserId);

            mockUserQueryHandler.Verify(uqh => uqh.GetAccounts(It.Is<string>(s => s.Equals(authUserId))));
            mockUserQueryHandler.VerifyNoOtherCalls();
        }
    }
}
