using System;
using Tests.Utilities.Composable;

namespace Tests.Utilities.Attributes
{
    internal interface IAttributeProcessor
    {
        public Type AttributeType { get; }
        public void Process(object obj, object attr);
    }
}
