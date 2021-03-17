using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Test
{
    public class AccountUserRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<DataContext> dbContextOptions;
        private readonly Shared.DatabaseSeeder databaseSeeder;

        private string AuthUserId => databaseSeeder.AuthUserId;
        private string OtherAuthUserId => databaseSeeder.OtherAuthUserId;
        private Entities.Account Account => databaseSeeder.Account;
        private Entities.Account OtherAccount => databaseSeeder.OtherAccount;

        private bool disposedValue;
        private readonly DataContext dbContext;
        private readonly FxCreditSystem.Repository.AccountUserRepository accountUserRepository;

        public AccountUserRepositoryTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite("Filename=FxCreditSystem.Repository.Test-AccountUser.db")
                .Options;

            dbContext = new DataContext(dbContextOptions);
            IMapper mapper = new MapperConfiguration(c => c.AddProfile<AutoMapperProfile>()).CreateMapper();
            accountUserRepository = new FxCreditSystem.Repository.AccountUserRepository(dbContext, mapper);

            databaseSeeder = new Shared.DatabaseSeeder(dbContext);
            databaseSeeder.Seed();
        }

        [Fact]
        public async Task Get_WithInvalidArguments_ShouldFail()
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await accountUserRepository.Get(null));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await accountUserRepository.Get(""));
        }

        [Fact]
        public async Task Get_WithUnknownUserId_ShouldFail()
        {
            string unknownUserId = Guid.NewGuid().ToString();

            var list = await accountUserRepository.Get(unknownUserId);
            Assert.Empty(list);
        }

        [Fact]
        public async Task Get_ShouldSucceed()
        {
            var list = await accountUserRepository.Get(AuthUserId);
            var accountUser = Assert.Single(list);
            Assert.Equal(Account.ExternalId, accountUser.AccountExternalId);
            Assert.Equal(Account.Description, accountUser.AccountDescription);
            Assert.Equal(AuthUserId, accountUser.AuthUserId);

            list = await accountUserRepository.Get(OtherAuthUserId);
            accountUser = Assert.Single(list);
            Assert.Equal(OtherAccount.ExternalId, accountUser.AccountExternalId);
            Assert.Equal(OtherAccount.Description, accountUser.AccountDescription);
            Assert.Equal(OtherAuthUserId, accountUser.AuthUserId);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TransactionRepositoryTest()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
