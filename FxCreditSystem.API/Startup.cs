using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace FxCreditSystem.API
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1", 
                    new OpenApiInfo
                    {
                        Title = "FxCreditSystem.API",
                        Version = "v1"
                    });

                //First we define the security scheme
                c.AddSecurityDefinition("Bearer", //Name the security scheme
                    new OpenApiSecurityScheme{
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http, //We set the scheme type to http since we're using bearer authentication
                    Scheme = "bearer" //The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement{ 
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "Bearer", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath, true);
            });

            services.AddCognitoIdentity();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = GetCognitoTokenValidationParams();

                    JwtBearerEvents authenticationEvents;
                    if (_configuration.GetValue("TroubleshootAuthentication", false))
                        authenticationEvents = GetTroubleshootAuthenticationEvents();
                    else
                        authenticationEvents = GetAuthenticationEvents();
                    options.Events = authenticationEvents;
                });

            services.AddAuthorization();

            services.AddAutoMapper(
                typeof(AutoMapperProfile), 
                typeof(FxCreditSystem.Repository.AutoMapperProfile));

            services.AddFxCreditSystemAPI();
            services.AddFxCreditSystemCore();
            services.AddFxCreditSystemRepository();

            services.AddDbContext<Repository.DataContext>(options =>
            {
                options.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => 
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FxCreditSystem.API v1");
                });
            }

            if (_configuration.GetValue("TroubleshootAuthentication", false))
                Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.EnsureMigrationOfContext<Repository.DataContext>();
            
        }

        private TokenValidationParameters GetCognitoTokenValidationParams()
        {
            var cognitoIssuer = $"https://cognito-idp.{_configuration["AWS:Region"]}.amazonaws.com/{_configuration["AWS:UserPoolId"]}";
            var jwtKeySetUrl = $"{cognitoIssuer}/.well-known/jwks.json";
            var cognitoAudience = _configuration["AWS:UserPoolClientId"];

            return new TokenValidationParameters
            {
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                {
                    // get JsonWebKeySet from AWS
                    var json = new WebClient().DownloadString(jwtKeySetUrl);
                    var keySet = new JsonWebKeySet(json);
                    var keys = keySet.Keys;
                    return keys;
                },
                ValidIssuer = cognitoIssuer,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateLifetime = true,
                // ValidAudience = cognitoAudience,
                ValidateAudience = false,
            };
        }

        private JwtBearerEvents GetAuthenticationEvents()
        {
            return new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    var loggerFactory = ctx.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<Startup>();

                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    logger.LogWarning("Authentication failed: " + FlattenException(ctx.Exception, false));
                    return Task.CompletedTask;
                }
            };
        }

        private JwtBearerEvents GetTroubleshootAuthenticationEvents()
        {
            return new JwtBearerEvents
            {
                OnAuthenticationFailed = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    ctx.HttpContext.Items["Message"] =
                        ctx.HttpContext.Items["Message"]
                        + "From OnAuthenticationFailed:\n"
                        + FlattenException(ctx.Exception, true) + "\n";
                    return Task.CompletedTask;
                },

                OnChallenge = ctx =>
                {
                    string message =
                        ctx.HttpContext.Items["Message"]
                        + "From OnChallenge:\n";
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    ctx.Response.ContentType = "text/plain";

                    var loggerFactory = ctx.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<Startup>();
                    logger.LogWarning(message);

                    return ctx.Response.WriteAsync(message);
                },

                OnMessageReceived = ctx =>
                {
                    ctx.Request.Headers.TryGetValue("Authorization", out var BearerToken);
                    if (BearerToken.Count == 0)
                        BearerToken = "no Bearer token sent\n";
                    ctx.HttpContext.Items["Message"] =
                        "From OnMessageReceived:\nAuthorization Header sent: " + BearerToken + "\n";
                    return Task.CompletedTask;
                },

                OnTokenValidated = ctx =>
                {
                    var loggerFactory = ctx.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger<Startup>();
                    logger.LogDebug("token: " + ctx.SecurityToken.ToString());
                    
                    return Task.CompletedTask;
                }
            };
        }

        public static string FlattenException(Exception exception, bool includeCallStack)
        {
            var stringBuilder = new StringBuilder();
            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);
                exception = exception.InnerException;
            }
            return stringBuilder.ToString();
        }
    }
}