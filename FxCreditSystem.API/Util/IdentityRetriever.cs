using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FxCreditSystem.API
{
    /// <summary>
    ///   Utility class to retrieve Authorization identity from Controller instance
    /// </summary>
    public interface IIdentityRetriever
    {
        /// <summary>
        ///   Retrieves Authorization identity from Controller instance
        /// </summary>
        string GetIdentity(ControllerBase controller);
    }

    [ExcludeFromCodeCoverage]
    internal class IdentityRetriever : IIdentityRetriever
    {
        public string GetIdentity(ControllerBase controller)
        {
            return controller.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}