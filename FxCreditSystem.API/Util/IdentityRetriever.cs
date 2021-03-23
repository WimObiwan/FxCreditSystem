using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FxCreditSystem.API
{
    public interface IIdentityRetriever
    {
        string GetIdentity(ControllerBase controller);
    }

    public class IdentityRetriever : IIdentityRetriever
    {
        public string GetIdentity(ControllerBase controller)
        {
            return controller.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}