using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Test
{
    public class UserRepositoryTest : Shared.TestBase
    {
        private Guid UserId => databaseSeeder.UserId;
        private Guid OtherUserId => databaseSeeder.OtherUserId;
        private Entities.Account Account => databaseSeeder.Account;
        private Entities.Account OtherAccount => databaseSeeder.OtherAccount;

        private readonly FxCreditSystem.Repository.UserRepository userRepository;

        public UserRepositoryTest() : base(nameof(UserRepositoryTest))
        {
            userRepository = new FxCreditSystem.Repository.UserRepository(dbContext, mapper);
        }

        [Fact]
        public async Task Get_WithUnknownUserId_ShouldFail()
        {
            Guid unknownUserId = Guid.NewGuid();

            var list = await userRepository.GetAccounts(unknownUserId);
            Assert.Empty(list);
        }

        [Fact]
        public async Task Get_ShouldSucceed()
        {
            var list = await userRepository.GetAccounts(UserId);
            var accountUser = Assert.Single(list);
            Assert.Equal(Account.ExternalId, accountUser.AccountId);
            Assert.Equal(Account.Description, accountUser.AccountDescription);
            Assert.Equal(UserId, accountUser.UserId);

            list = await userRepository.GetAccounts(OtherUserId);
            accountUser = Assert.Single(list);
            Assert.Equal(OtherAccount.ExternalId, accountUser.AccountId);
            Assert.Equal(OtherAccount.Description, accountUser.AccountDescription);
            Assert.Equal(OtherUserId, accountUser.UserId);
        }
    }
}
