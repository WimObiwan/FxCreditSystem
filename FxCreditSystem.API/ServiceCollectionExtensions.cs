using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace FxCreditSystem.API
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFxCreditSystemAPI(this IServiceCollection services)
        {
            services.AddTransient<API.IIdentityRetriever, API.IdentityRetriever>();
            return services;
        }

        public static IServiceCollection AddFxCreditSystemCore(this IServiceCollection services)
        {
            services.AddTransient<Common.IUserQueryHandler, Core.UserQueryHandler>();
            services.AddTransient<Core.IAuthorizationService, Core.AuthorizationService>();
            return services;
        }

        public static IServiceCollection AddFxCreditSystemRepository(this IServiceCollection services)
        {
            services.AddTransient<Common.IUserRepository, Repository.UserRepository>();
            return services;
        }
    }
}