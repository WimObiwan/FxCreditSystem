using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    [ExcludeFromCodeCoverage]
    internal class ZabbixFriendlyHealthCheckWriter : IHealthCheckWriter
    {
        readonly JsonSerializerOptions _jsonSerializerOptions;

        public ZabbixFriendlyHealthCheckWriter()
        {
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new VersionJsonConverter());
        }

        public async Task WriteResponse(HttpContext httpContext, HealthReport report)
        {
            httpContext.Response.ContentType = "application/json";

            IList<ResponseResultEntry> entries;
            if (report.Entries?.Any() ?? false)
                entries = report.Entries.Select(e => 
                    new ResponseResultEntry()
                    {
                        Name = e.Key,
                        Status = (int)e.Value.Status,
                        StatusText = e.Value.Status.ToString(),
                        Description = e.Value.Description,
                        Data = (e.Value.Data?.Any() ?? false) ? e.Value.Data : null,
                        Exception = e.Value.Exception,
                        Duration = e.Value.Duration.TotalMilliseconds
                    }
                ).ToList();
            else
                entries = null;

            var response = new Response()
            {
                DateTime = DateTime.UtcNow,
                Status = (int)report.Status,
                StatusText = report.Status.ToString(),
                Results = entries,
                TotalDuration = report.TotalDuration.TotalMilliseconds
            };

            string json = JsonSerializer.Serialize<Response>(response, _jsonSerializerOptions);
            await httpContext.Response.WriteAsync(json);
        }

        private class Response
        {
            [JsonPropertyName("dateTime")]
            public DateTime DateTime { get; set; }

            [JsonPropertyName("status")]
            public int Status { get; set; }

            [JsonPropertyName("statusText")]
            public string StatusText { get; set; }
             
            [JsonPropertyName("results")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public IList<ResponseResultEntry> Results { get; set; }

            [JsonPropertyName("totalDuration_ms")]
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
            public IReadOnlyDictionary<string, object> Data { get; set; }
            
            [JsonPropertyName("duration_ms")]
            public double Duration { get; set; }

            [JsonPropertyName("exception")]
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public Exception Exception { get; set; }
        }

        private class VersionJsonConverter : JsonConverter<Version>
        {
            public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString());
            }
        }
    }
}