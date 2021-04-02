
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FxCreditSystem.API
{
    [ExcludeFromCodeCoverage]
    internal class SystemInfoHealthCheck : IHealthCheck
    {
        private readonly IConfiguration configuration;
        private readonly string _version;
        private readonly DateTime _buildTimestamp;
        private readonly string _path;

        public SystemInfoHealthCheck(IConfiguration configuration)
        {
            this.configuration = configuration;
            
            _version = $"{ThisAssembly.Git.SemVer.Major}.{ThisAssembly.Git.SemVer.Minor}.{ThisAssembly.Git.Commits}";
            #pragma warning disable CS0162
            if (ThisAssembly.Git.Branch != "master" && ThisAssembly.Git.Branch != "main")
                _version += $"-{ThisAssembly.Git.Branch}";
            #pragma warning restore CS0162
            if (ThisAssembly.Git.Commits != "0")
                _version += $"+{ThisAssembly.Git.Commit}";
            _path = Path.GetDirectoryName(GetType().Assembly.Location);
            _buildTimestamp = File.GetLastWriteTimeUtc(GetType().Assembly.Location);
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = new Dictionary<string, object>();
            data["dotnet"] = Environment.Version;
            data["version"] = _version;
            data["server"] = Environment.MachineName;
            data["path"] = _path;
            data["buildTimestamp"] = _buildTimestamp;
            data["processStartTimestamp"] = Process.GetCurrentProcess().StartTime.ToUniversalTime();
            data["memory"] = GC.GetTotalMemory(false);
            var result = new HealthCheckResult(HealthStatus.Healthy, data: data);
            return Task.FromResult(result);
        }
    }
}