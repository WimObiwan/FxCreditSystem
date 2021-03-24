using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FxCreditSystem.API
{
    public interface IIdentityRetriever
    {
        string GetIdentity(ControllerBase controller);
    }

    [ExcludeFromCodeCoverage]
    public class IdentityRetriever : IIdentityRetriever
    {
        public string GetIdentity(ControllerBase controller)
        {
            return controller.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}