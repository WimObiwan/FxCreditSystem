using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FxCreditSystem.API.Controllers;
using FxCreditSystem.API.DTO;
using FxCreditSystem.Common;
using FxCreditSystem.Common.Fakers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FxCreditSystem.API.Test
{
    public class UserControllerTest
    {
        [Fact]
        public async Task GetAccounts_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            string authUserId = $"test|{faker.Random.Hexadecimal(16, "")}";
            var accountUserFaker = new AccountUserFaker().RuleFor(au => au.AuthUserId, authUserId);
            var accountUser = accountUserFaker.Generate();

            var logger = new Mock<ILogger<UserController>>();
            var mapper = new MapperConfiguration(c => c.AddProfile<AutoMapperProfile>()).CreateMapper();
            var mockUserQueryHandler = new Mock<IUserQueryHandler>();
            mockUserQueryHandler.Setup(uqh => uqh.GetAccounts(authUserId)).ReturnsAsync(new [] { accountUser });
            var userController = new UserController(logger.Object, mapper, mockUserQueryHandler.Object);
            
            var result = await userController.Get(authUserId);

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var accountUserResponse = Assert.IsType<List<AccountUserResponse>>(okObjectResult.Value);

            mockUserQueryHandler.Verify(uqh => uqh.GetAccounts(It.Is<string>(s => s.Equals(authUserId))));
            mockUserQueryHandler.VerifyNoOtherCalls();
        }
    }
}
