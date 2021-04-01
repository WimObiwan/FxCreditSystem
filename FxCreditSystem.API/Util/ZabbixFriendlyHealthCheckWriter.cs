using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FxCreditSystem.API
{
    internal interface IHealthCheckWriter
    {
        Task WriteResponse(HttpContext httpContext, HealthReport report);
    }

    internal class ZabbixFriendlyHealthCheckWriter : IHealthCheckWriter
    {
        private class Response
        {
            [JsonPropertyName("dateTime")]
            public DateTime DateTime { get; set; }

            [JsonPropertyName("status")]
            public int Status { get; set; }

            [JsonPropertyName("statusText")]
            public string StatusText { get; set; }
             
            [JsonPropertyName("results")]
            public IList<ResponseResultEntry> Results { get; set; }

            [JsonPropertyName("totalDuration")]
            public double TotalDuration { get; set; }
        }

        private class ResponseResultEntry
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("status")]
            public int Status { get; set; }

            [JsonPropertyName("statusText")]
            public string StatusText { get; set; }

            [JsonPropertyName("description")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Description { get; set; }

            [JsonPropertyName("data")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public Dictionary<string, string> Data { get; set; }
            
            [JsonPropertyName("duration")]
            public double Duration { get; set; }
        }

        public async Task WriteResponse(HttpContext httpContext, HealthReport report)
        {
            httpContext.Response.ContentType = "application/json";

            var response = new Response()
            {
                DateTime = DateTime.UtcNow,
                Status = (int)report.Status,
                StatusText = report.Status.ToString(),
                Results = report.Entries.Select(e => 
                    new ResponseResultEntry()
                    {
                        Name = e.Key,
                        Status = (int)e.Value.Status,
                        StatusText = e.Value.Status.ToString(),
                        Description = e.Value.Description,
                        Data = e.Value.Data.ToDictionary(d => d.Key, d => d.Value.ToString()),
                        Duration = e.Value.Duration.TotalMilliseconds
                    }
                ).ToList(),
                TotalDuration = report.TotalDuration.TotalMilliseconds
            };

            string json = JsonSerializer.Serialize<Response>(response);
            await httpContext.Response.WriteAsync(json);
        }
    }    
}