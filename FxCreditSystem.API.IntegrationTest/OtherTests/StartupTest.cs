using FxCreditSystem.API.Controllers;
using FxCreditSystem.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FxCreditSystem.API.Test
{
    public class StartupTest
    {
        [Fact]
        public void ConfigureServices_ShouldSucceed()
        {
            // 
            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();
        }
    }
}
