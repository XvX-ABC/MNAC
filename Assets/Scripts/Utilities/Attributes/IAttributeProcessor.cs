using System;
using MNAC.Utilities.Composable;

namespace MNAC.Utilities.Attributes
{
    public interface IAttributeProcessor
    {
        public Type AttributeType { get; }
        public void Process(object obj, object attr);
    }
}
