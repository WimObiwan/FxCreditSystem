using System;
using System.Linq;

namespace FxCreditSystem.API.Repositories
{
    interface IRepository<T>
    {
        T Get(Guid id);
        IQueryable<T> GetAll();
    }
}