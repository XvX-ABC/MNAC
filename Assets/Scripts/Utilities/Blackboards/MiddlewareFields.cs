using System;

namespace MNAC.Utilities.Blackboards
{
    public static class MiddlewareFields
    {
        static MiddlewareFields()
        {
            FieldChangeHandler = Guid.NewGuid();
        }
        public static readonly Guid FieldChangeHandler;
    }
}
