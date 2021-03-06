using System;
using System.Linq;

namespace FxCreditSystem.Api.Repositories
{
    interface IRepository<T>
    {
        T Get(Guid id);
        IQueryable<T> GetAll();
    }
}