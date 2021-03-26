using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Common.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string message) : base(message)
        {}
    }
}