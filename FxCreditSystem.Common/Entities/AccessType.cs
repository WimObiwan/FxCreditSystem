using System;

namespace FxCreditSystem.Common.Entities
{

    [Flags]
    public enum AccessType
    {
        Read = 1,
        Write = 2,
        Any = int.MaxValue,
    }
}
