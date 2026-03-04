using System;

namespace MNAC.Utilities.Attributes
{
    internal class PlayerComponentAttributeProcessor : IAttributeProcessor
    {
        DontDestroyOnLoadAttributeProcessor _processor;
        public PlayerComponentAttributeProcessor(DontDestroyOnLoadAttributeProcessor processor)
        {
            _processor = processor;
        }
        public Type AttributeType => typeof(PlayerComponentAttribute);

        public void Process(object obj, object attr)
        {
            if (attr is not PlayerComponentAttribute pattr)
                return;
            if (pattr.DontDestroyOnLoad)
                _processor.Process(obj, pattr);
        }
    }
}
