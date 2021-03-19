using System.Collections.Generic;
using System.Threading.Tasks;

namespace FxCreditSystem.Common
{
    public interface IUserQueryHandler
    {
        Task<IList<Common.Entities.AccountUser>> GetAccounts(string authUserId);
    }
}
