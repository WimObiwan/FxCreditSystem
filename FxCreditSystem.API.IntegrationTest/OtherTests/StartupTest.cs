using FxCreditSystem.API.Controllers;
using FxCreditSystem.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FxCreditSystem.API.IntegrationTest
{
    public class StartupTest
    {
        [Fact]
        public void ConfigureServices_ShouldSucceed()
        {
            // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests
            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();
        }
    }
}
