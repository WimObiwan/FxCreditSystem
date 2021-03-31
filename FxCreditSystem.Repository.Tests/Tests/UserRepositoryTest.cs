using System;
using System.Threading.Tasks;
using AutoMapper;
using FxCreditSystem.Common.Fakers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Tests
{
    public class UserRepositoryTest : Shared.TestBase
    {
        private Guid UserId => databaseSeeder.User.ExternalId;
        private Guid OtherUserId => databaseSeeder.OtherUser.ExternalId;
        private string UserIdentity => databaseSeeder.User.Identities[0].Identity;
        private string OtherUserIdentity => databaseSeeder.OtherUser.Identities[0].Identity;
        private Entities.Account Account => databaseSeeder.Account;
        private Entities.Account OtherAccount => databaseSeeder.OtherAccount;

        private readonly FxCreditSystem.Repository.UserRepository userRepository;

        public UserRepositoryTest() : base(nameof(UserRepositoryTest))
        {
            userRepository = new FxCreditSystem.Repository.UserRepository(dbContext, mapper);
        }

        [Fact]
        public async Task HasIdentity_ShouldSucceed()
        {
            var result = await userRepository.HasIdentity(UserId, UserIdentity);
            Assert.True(result);

            result = await userRepository.HasIdentity(OtherUserId, OtherUserIdentity);
            Assert.True(result);
        }

        [Fact]
        public async Task HasIdentity_WithoutIdentity_ShouldSucceed()
        {
            var result = await userRepository.HasIdentity(UserId, OtherUserIdentity);
            Assert.False(result);

            result = await userRepository.HasIdentity(OtherUserId, UserIdentity);
            Assert.False(result);
        }

        [Fact]
        public async Task HasIdentity_WithUnknownUser_ShouldFail()
        {
            Bogus.Faker faker = new Bogus.Faker();
            var identity = faker.Random.Identity();
            var userId = faker.Random.Guid();

            await Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await userRepository.HasIdentity(userId, identity));
        }

        [Fact]
        public async Task GetIdentities_WithUnknownUserId_ShouldFail()
        {
            Guid unknownUserId = Guid.NewGuid();

            await Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await userRepository.GetIdentities(unknownUserId));
        }

        [Fact]
        public async Task GetIdentities_ShouldSucceed()
        {
            var list = await userRepository.GetIdentities(UserId);
            var userIdentity = Assert.Single(list, ui => ui.Identity == UserIdentity);
            Assert.Equal(UserId, userIdentity.UserId);
            Assert.Equal(UserIdentity, userIdentity.Identity);

            list = await userRepository.GetIdentities(OtherUserId);
            userIdentity = Assert.Single(list, ui => ui.Identity == OtherUserIdentity);
            Assert.Equal(OtherUserId, userIdentity.UserId);
            Assert.Equal(OtherUserIdentity, userIdentity.Identity);
        }

        [Fact]
        public async Task GetAccounts_WithUnknownUserId_ShouldFail()
        {
            Guid unknownUserId = Guid.NewGuid();

            await Assert.ThrowsAsync<UserNotFoundException>(async () =>
                await userRepository.GetAccounts(unknownUserId));
        }

        [Fact]
        public async Task GetAccounts_ShouldSucceed()
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
