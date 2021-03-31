using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository.Tests.Shared
{
    public class TestBase : IDisposable
    {
        protected readonly DbContextOptions<DataContext> dbContextOptions;
        protected readonly Shared.DatabaseSeeder databaseSeeder;
        protected readonly IMapper mapper;

        private bool disposedValue;
        protected readonly DataContext dbContext;

        public TestBase(string className)
        {
            dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite($"Filename=FxCreditSystem.Repository.Test-{className}.db")
                .Options;

            dbContext = new DataContext(dbContextOptions);
            mapper = new MapperConfiguration(c => c.AddProfile<AutoMapperProfile>()).CreateMapper();

            databaseSeeder = new Shared.DatabaseSeeder(dbContext);
            databaseSeeder.Seed();
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

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}