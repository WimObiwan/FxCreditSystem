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

namespace FxCreditSystem.API.Tests
{
    public class UserControllerTest
    {
        [Fact]
        public async Task GetIdentities_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var userId = faker.Random.Guid();
            var userIdentityFaker = new UserIdentityFaker().RuleFor(au => au.UserId, userId);
            var userIdentity = userIdentityFaker.Generate();

            var logger = new Mock<ILogger<UserController>>();
            var mapper = new MapperConfiguration(c => c.AddProfile<AutoMapperProfile>()).CreateMapper();

            var mockIdentityRetriever = new Mock<IIdentityRetriever>();
            mockIdentityRetriever.Setup(uqh => uqh.GetIdentity(It.IsAny<ControllerBase>())).Returns(identity);

            var mockUserQueryHandler = new Mock<IUserQueryHandler>();
            mockUserQueryHandler.Setup(uqh => uqh.GetIdentities(identity, userId)).ReturnsAsync(new [] { userIdentity });

            var userController = new UserController(logger.Object, mapper, mockIdentityRetriever.Object, mockUserQueryHandler.Object);

            var result = await userController.GetIdentities(userId);

            var actionResult = Assert.IsType<ActionResult<IList<UserIdentityResponse>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var userIdentityResponses = Assert.IsType<List<UserIdentityResponse>>(okObjectResult.Value);
            var userIdentityResponse = userIdentityResponses[0];
            Assert.Equal(userIdentity.UserId, userIdentityResponse.UserId);
            Assert.Equal(userIdentity.Identity, userIdentityResponse.Identity);

            mockUserQueryHandler.Verify(uqh => uqh.GetIdentities(It.Is<string>(s => s.Equals(identity)), It.Is<Guid>(s => s.Equals(userId))));
            mockUserQueryHandler.VerifyNoOtherCalls();
        }

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

            var result = await userController.GetAccounts(userId);

            var actionResult = Assert.IsType<ActionResult<IList<AccountUserResponse>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var accountUserResponses = Assert.IsType<List<AccountUserResponse>>(okObjectResult.Value);
            var accountUserResponse = Assert.Single(accountUserResponses);
            Assert.Equal(accountUser.AccountId, accountUserResponse.AccountId);
            Assert.Equal(accountUser.AccountDescription, accountUserResponse.AccountDescription);
            Assert.Equal(accountUser.UserId, accountUserResponse.UserId);
            Assert.Equal(accountUser.UserDescription, accountUserResponse.UserDescription);

            mockUserQueryHandler.Verify(uqh => uqh.GetAccounts(It.Is<string>(s => s.Equals(identity)), It.Is<Guid>(s => s.Equals(userId))));
            mockUserQueryHandler.VerifyNoOtherCalls();
        }
    }
}
