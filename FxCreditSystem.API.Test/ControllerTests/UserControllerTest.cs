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

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var accountUserResponse = Assert.IsType<List<UserIdentityResponse>>(okObjectResult.Value);
            var accountUserResponse0 = accountUserResponse[0];
            Assert.Equal(userIdentity.UserId, accountUserResponse0.UserId);
            Assert.Equal(userIdentity.Identity, accountUserResponse0.Identity);

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

            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var accountUserResponse = Assert.IsType<List<AccountUserResponse>>(okObjectResult.Value);
            Assert.Equal(accountUser.AccountId, accountUserResponse[0].AccountId);
            Assert.Equal(accountUser.AccountDescription, accountUserResponse[0].AccountDescription);
            Assert.Equal(accountUser.UserId, accountUserResponse[0].UserId);
            Assert.Equal(accountUser.UserDescription, accountUserResponse[0].UserDescription);

            mockUserQueryHandler.Verify(uqh => uqh.GetAccounts(It.Is<string>(s => s.Equals(identity)), It.Is<Guid>(s => s.Equals(userId))));
            mockUserQueryHandler.VerifyNoOtherCalls();
        }
    }
}
