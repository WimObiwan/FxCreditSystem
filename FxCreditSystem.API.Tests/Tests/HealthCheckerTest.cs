using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace FxCreditSystem.API.Tests
{
    public class SystemInfoHealthCheckTest
    {
        [Fact]
        public async Task CheckHealthAsync_ShouldSucceed()
        {
            var systemInfoHealthCheck = new SystemInfoHealthCheck();
            var healthCheckContext = new HealthCheckContext();
            var result = await systemInfoHealthCheck.CheckHealthAsync(healthCheckContext);
            Assert.NotNull(result);
            Assert.Null(result.Description);
            Assert.Null(result.Exception);
            Assert.Equal(HealthStatus.Healthy, result.Status);
            Assert.NotNull(result.Data);     
            // Data...
        }
    }
}
