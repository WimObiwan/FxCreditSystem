using Microsoft.Extensions.DependencyInjection;

namespace FxCreditSystem.API
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFxCreditSystemCore(this IServiceCollection services)
        {
            services.AddTransient<FxCreditSystem.Common.IUserQueryHandler, FxCreditSystem.Core.UserQueryHandler>();
            return services;
        }

        public static IServiceCollection AddFxCreditSystemRepository(this IServiceCollection services)
        {
            services.AddTransient<FxCreditSystem.Common.IAccountUserRepository, FxCreditSystem.Repository.AccountUserRepository>();
            return services;
        }
    }
}