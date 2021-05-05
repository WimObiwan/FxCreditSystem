using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Reflection;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Microsoft.AspNetCore.HttpOverrides;

namespace FxCreditSystem.API
{
    [ExcludeFromCodeCoverage]
    internal class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-5.0#forward-the-scheme-for-linux-and-non-iis-reverse-proxies
            if (_configuration.GetValue<bool>("ForwardedHeadersEnabled", false))
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                    {
                        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    });
            }

            services.AddControllers(c =>
            {
                c.Filters.Add(new ConsumesAttribute("application/json"));
                c.Filters.Add(new ProducesAttribute("application/json"));
                c.Filters.Add(new ProducesResponseTypeAttribute((int)HttpStatusCode.OK));
                c.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest));
                c.Filters.Add(new ProducesResponseTypeAttribute((int)HttpStatusCode.Unauthorized));
                c.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), (int)HttpStatusCode.NotFound));
                c.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError));
            });

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

                var authorizationURL = _configuration.GetValue("Swagger:AuthorizationURL", "");
                if (string.IsNullOrEmpty(authorizationURL))
                {
                    c.AddSecurityDefinition("Bearer",
                        new OpenApiSecurityScheme{
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement{ 
                        {
                            new OpenApiSecurityScheme{
                                Reference = new OpenApiReference{
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            }, new List<string>()
                        }
                    });
                }
                else
                {
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Description = "OAuth2 Implicit flow.",
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(authorizationURL),
                                //TokenUrl = new Uri("/auth-server/connect/token", UriKind.Relative),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "openid", "openid" },
                                },
                            }
                        }
                    });
                    
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement{ 
                        {
                            new OpenApiSecurityScheme{
                                Reference = new OpenApiReference{
                                    Id = "oauth2", //The name of the previously defined security scheme.
                                    Type = ReferenceType.SecurityScheme
                                }
                            }, new List<string>()
                        }
                    });
                }

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

            var healthCheckBuilder = services.AddHealthChecks()
                .AddCheck<SystemInfoHealthCheck>("systeminfo");
            string databaseType = _configuration.GetValue<string>("DatabaseType", null);
            if (databaseType == "Sqlite")
            {
                healthCheckBuilder
                    .AddSqlite(
                        sqliteConnectionString: _configuration.GetConnectionString("DefaultConnection"), 
                        name: "sqlite-Default");
            }
            else if (databaseType == "SqlServer")
            {
                healthCheckBuilder
                    .AddSqlServer(
                        connectionString: _configuration.GetConnectionString("DefaultConnection"),
                        name: "sqlserver-Default");
            }
            else
            {
                throw new InvalidOperationException($"Unknown DatabaseType in configuration ({databaseType})");
            }
            services.AddSingleton<IHealthCheckWriter, ZabbixFriendlyHealthCheckWriter>();

            services.AddAutoMapper(
                typeof(AutoMapperProfile), 
                typeof(FxCreditSystem.Repository.AutoMapperProfile));

            services.AddFxCreditSystemAPI();
            services.AddFxCreditSystemCore();
            services.AddFxCreditSystemRepository();

            services.AddDbContext<Repository.DataContext>(options =>
            {
                if (databaseType == "Sqlite")
                {
                    options.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));
                }
                else if (databaseType == "SqlServer")
                {
                    options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
                }
                else
                {
                    throw new InvalidOperationException($"Unknown DatabaseType in configuration ({databaseType})");
                }
            });

            // Rate limit
            services.AddOptions();
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>(_configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(_configuration.GetSection("IpRateLimitPolicies"));
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHealthCheckWriter healthCheckWriter)
        {
            // Detailed Exception info in Development environment is handled by ErrorHandlerMiddleware

            app.UseIpRateLimiting();
            app.UseSwagger(c =>
                c.RouteTemplate = "openapi/{documentName}/openapi.json"
            );
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/openapi/v1/openapi.json", "FxCreditSystem.API v1");
                c.OAuthClientId(_configuration["AWS:UserPoolClientId"]);
                c.OAuth2RedirectUrl("https://localhost:5001/openapi/oauth2-redirect.html");
                c.RoutePrefix = "openapi";
            });

            if (_configuration.GetValue("TroubleshootAuthentication", false))
                Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            // HealthCheck middleware
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = healthCheckWriter.WriteResponse
            });

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
                    var exceptionFormatter = ctx.HttpContext.RequestServices
                        .GetRequiredService<IExceptionFormatter>();

                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    logger.LogWarning("Authentication failed: " + exceptionFormatter.GetText(ctx.Exception, false));
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
                    var exceptionFormatter = ctx.HttpContext.RequestServices
                        .GetRequiredService<IExceptionFormatter>();

                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    ctx.HttpContext.Items["Message"] =
                        ctx.HttpContext.Items["Message"]
                        + "From OnAuthenticationFailed:\n"
                        + exceptionFormatter.GetText(ctx.Exception, true) + "\n";
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
    }
}
