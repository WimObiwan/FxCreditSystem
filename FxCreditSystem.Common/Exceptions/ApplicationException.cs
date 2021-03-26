using System;
using System.Runtime.Serialization;

namespace FxCreditSystem.Common.Exceptions
{
    public class ApplicationException : Exception
    {
        public ApplicationException(string message) : base(message)
        {}
    }
}