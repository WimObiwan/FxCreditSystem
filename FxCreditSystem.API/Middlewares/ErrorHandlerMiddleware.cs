using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace FxCreditSystem.API
{
    [ExcludeFromCodeCoverage]
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                HttpStatusCode statusCode;
                if (ex is Common.Exceptions.NotFoundException)
                    statusCode = HttpStatusCode.NotFound;
                else if (ex is Common.Exceptions.ApplicationException)
                    statusCode = HttpStatusCode.BadRequest;
                else
                    statusCode = HttpStatusCode.InternalServerError;

                var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
                bool isDevelopment = env.IsDevelopment() || env.IsStaging();

                var exceptionFormatter = context.RequestServices
                    .GetRequiredService<IExceptionFormatter>();
                string detail = ex.Message; 
                if (isDevelopment)
                {
                    detail += "\n\n" + ex.StackTrace;
                }

                // https://tools.ietf.org/html/rfc7807
                var result = JsonSerializer.Serialize(new ProblemDetails {
                     Title = ex.GetType().Name,
                     Detail = ex.Message,
                     Status = (int)statusCode                     
                });

                var response = context.Response;
                response.StatusCode = (int)statusCode;
                response.ContentType = "application/json";
                await response.WriteAsync(result);
            }
        }
    }
}