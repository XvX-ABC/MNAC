using System.Collections.Generic;

namespace Tests.Blackboards
{
    public interface IMiddleware<K, A>
    {
        public bool Enabled { get; set; }
        public IReadOnlyDictionary<K, object> Values { set; }
        public void Initialize(Blackboard<K, A> blackboard);
        public (bool, object) ValueRegisterHandle(ValueInfo<K, A> dataInfo);
        public (bool, object) ValueUnregisterHandle(ValueInfo<K, A> dataInfo);
        public (bool, object) ValueReadingHandle(ValueInfo<K, A> dataInfo);
        public (bool, object) ValueWritingHandle(ValueInfo<K, A> dataInfo);
    }
}
