using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DeepEqual.Syntax;
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
    public class AccountControllerTest
    {
        [Fact]
        public async Task GetAccount_ShouldSucceed()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();

            var accountId = faker.Random.Guid();
            var accountFaker = new AccountFaker().RuleFor(a => a.Id, accountId);
            var account = accountFaker.Generate();

            var logger = new Mock<ILogger<UserController>>();
            var mapper = new MapperConfiguration(c => c.AddProfile<AutoMapperProfile>()).CreateMapper();

            var mockIdentityRetriever = new Mock<IIdentityRetriever>();
            mockIdentityRetriever.Setup(uqh => uqh.GetIdentity(It.IsAny<ControllerBase>())).Returns(identity);

            var mockAccountQueryHandler = new Mock<IAccountQueryHandler>();
            mockAccountQueryHandler.Setup(aqh => aqh.GetAccount(identity, accountId)).ReturnsAsync(account);

            var accountController = new AccountController(logger.Object, mapper, mockIdentityRetriever.Object, mockAccountQueryHandler.Object);

            var result = await accountController.GetAccount(accountId);

            var actionResult = Assert.IsType<ActionResult<AccountResponse>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var accountResponse = Assert.IsType<AccountResponse>(okObjectResult.Value);
            Assert.Equal(accountId, accountResponse.Id);
            accountResponse.ShouldDeepEqual(account);

            mockAccountQueryHandler.Verify(aqh => aqh.GetAccount(It.Is<string>(s => s.Equals(identity)), It.Is<Guid>(s => s.Equals(accountId))));
            mockAccountQueryHandler.VerifyNoOtherCalls();
        }
    }
}
