using System;
using System.Linq;

namespace MNAC.Utilities.Attributes
{
    public class AttributeProcessingCore
    {
        static AttributeProcessingCore s_instance;
        static AttributeProcessingCore()
        {
            var ddolProcessor = new DontDestroyOnLoadAttributeProcessor();
            var playerProcessor = new PlayerComponentAttributeProcessor(ddolProcessor);
            s_instance = new(
                playerProcessor,
                ddolProcessor
                );
        }

        IAttributeProcessor[] _processor;

        internal static AttributeProcessingCore Instance { get => s_instance; }
        public static void Process(object obj)
        {
            s_instance.ProcessImpl(obj);
        }

        public AttributeProcessingCore(params IAttributeProcessor[] handlers)
        {
            _processor = handlers;
        }
        internal void ProcessImpl(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            var type = obj.GetType();
            var attributes = type.GetCustomAttributes(true);
            foreach (var processor in _processor)
            {
                var attr = processor.AttributeType;
                if (processor == null || !attributes.Contains(attr))
                    continue;
                processor.Process(obj, attr);
            }
        }
    }
}
