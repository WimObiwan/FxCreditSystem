using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FxCreditSystem.Common
{
    public interface IAccountUserRepository
    {
        Task<IList<Common.Entities.AccountUser>> Get(string authUserId);
    }
}
