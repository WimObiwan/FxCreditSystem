using System;
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
            var identity = faker.Random.Identity();

            var userId = faker.Random.Guid();
            var accountUserFaker = new AccountUserFaker().RuleFor(au => au.UserId, userId);
            var accountUser = accountUserFaker.Generate();

            var logger = new Mock<ILogger<UserController>>();
            var mapper = new MapperConfiguration(c => c.AddProfile<AutoMapperProfile>()).CreateMapper();

            var mockIdentityRetriever = new Mock<IIdentityRetriever>();
            mockIdentityRetriever.Setup(uqh => uqh.GetIdentity(It.IsAny<ControllerBase>())).Returns(identity);

            var mockUserQueryHandler = new Mock<IUserQueryHandler>();
            mockUserQueryHandler.Setup(uqh => uqh.GetAccounts(identity, userId)).ReturnsAsync(new [] { accountUser });

            var userController = new UserController(logger.Object, mapper, mockIdentityRetriever.Object, mockUserQueryHandler.Object);

            var result = await userController.Get(userId);

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var accountUserResponse = Assert.IsType<List<AccountUserResponse>>(okObjectResult.Value);

            mockUserQueryHandler.Verify(uqh => uqh.GetAccounts(It.Is<string>(s => s.Equals(identity)), It.Is<Guid>(s => s.Equals(userId))));
            mockUserQueryHandler.VerifyNoOtherCalls();
        }
    }
}
