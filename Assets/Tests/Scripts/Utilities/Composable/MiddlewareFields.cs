using System;

namespace Tests.Utilities.Composable
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
