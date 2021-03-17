using System;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;

namespace FxCreditSystem.Repository
{
    public class DatabaseException : Exception
    {
        public DatabaseException(DbUpdateException x) 
            : base ("Database operation failed", x)
        {
        }
    }
}