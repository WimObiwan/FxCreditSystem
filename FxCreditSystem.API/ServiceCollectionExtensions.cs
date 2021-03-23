using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace FxCreditSystem.API
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFxCreditSystemAPI(this IServiceCollection services)
        {
            services.AddTransient<FxCreditSystem.API.IIdentityRetriever, FxCreditSystem.API.IdentityRetriever>();
            return services;
        }

        public static IServiceCollection AddFxCreditSystemCore(this IServiceCollection services)
        {
            services.AddTransient<FxCreditSystem.Common.IUserQueryHandler, FxCreditSystem.Core.UserQueryHandler>();
            return services;
        }

        public static IServiceCollection AddFxCreditSystemRepository(this IServiceCollection services)
        {
            services.AddTransient<FxCreditSystem.Common.IUserRepository, FxCreditSystem.Repository.UserRepository>();
            return services;
        }
    }
}