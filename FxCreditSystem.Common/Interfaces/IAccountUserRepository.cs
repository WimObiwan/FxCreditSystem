using System.Collections.Generic;
using System.Threading.Tasks;
using FxCreditSystem.Common.Entities;

namespace FxCreditSystem.Common
{
    public interface IAccountUserRepository
    {
        Task<IList<AccountUser>> Get(string authUserId);
    }
}
