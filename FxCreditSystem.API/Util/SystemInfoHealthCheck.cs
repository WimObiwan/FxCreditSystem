
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FxCreditSystem.API
{
    internal class SystemInfoHealthCheck : IHealthCheck
    {
        private IConfiguration configuration;

        public SystemInfoHealthCheck(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = new Dictionary<string, object>();
            data["dotnet"] = Environment.Version.ToString();
            var result = new HealthCheckResult(HealthStatus.Healthy, data: data);
            return Task.FromResult(result);
        }
    }
}