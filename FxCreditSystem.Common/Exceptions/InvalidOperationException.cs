using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Common.Exceptions
{
    public class InvalidOperationException : ApplicationException
    {
        public InvalidOperationException(string message) : base(message)
        {}
    }
}