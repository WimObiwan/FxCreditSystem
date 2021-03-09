using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FxCreditSystem.Repository.Test
{
    public class AccountUserRepositoryTest : IDisposable
    {
        private readonly DbContextOptions<DataContext> dbContextOptions;

        private string authUserId;
        private string otherAuthUserId;
        private long accountInternalId;
        private Guid accountId;
        private long otherAccountInternalId;
        private Guid otherAccountId;

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

            Seed();
        }

        private void Seed()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            authUserId = $"Test|{Guid.NewGuid().ToString()}";
            otherAuthUserId = $"Test|{Guid.NewGuid().ToString()}";
            var user = new Entities.User()
            {
                AuthUserId = authUserId,
                Description = "Test",                        
            };
            var otherUser = new Entities.User()
            {
                AuthUserId = otherAuthUserId,
                Description = "Test Other",                        
            };
            dbContext.Users.AddRange(
                user,
                otherUser
            );

            accountId = Guid.NewGuid();
            otherAccountId = Guid.NewGuid();
            var account = new Entities.Account {
                ExternalId = accountId,
                Description = "Account 1",
                MinimumCredits = -10.0m,
                Credits = 100.0m,
            };
            var otherAccount = new Entities.Account {
                ExternalId = otherAccountId,
                Description = "Account 2",
                MinimumCredits = -20.0m,
                Credits = 120.0m,
            };
            dbContext.Accounts.AddRange(
                account,
                otherAccount
            );

            dbContext.AccountUsers.AddRange(
                new Entities.AccountUser
                {
                    Account = account,
                    User = user,
                },
                new Entities.AccountUser
                {
                    Account = otherAccount,
                    User = otherUser,
                }
            );

            dbContext.SaveChanges();

            accountInternalId = account.Id;
            otherAccountInternalId = otherAccount.Id;
        }

        [Fact]
        public async Task Get_WithInvalidArguments_ShouldFail()
        {
            Guid transactionId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;

            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await accountUserRepository.Get(null));
            await Assert.ThrowsAsync<ArgumentException>(async () => 
                await accountUserRepository.Get(""));
        }

        [Fact]
        public async Task Get_ShouldSucceed()
        {
            var list = await accountUserRepository.Get(authUserId);
            Assert.Single(list);

            list = await accountUserRepository.Get(otherAuthUserId);
            Assert.Single(list);
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
