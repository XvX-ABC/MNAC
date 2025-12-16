using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Interaction.Influence
{
    public class InfluenceCore
    {
        Dictionary<Type, IInfluence> _influences;
        public InfluenceCore(params IInfluence[] influences)
        {
            this._influences = new();
            foreach (var i in influences)
            {
                AddInfluence(i);
            }
        }
        public void AddInfluence(IInfluence influence)
        {
            if (influence == null)
                throw new ArgumentNullException(nameof(influence));
            var type = influence.GetType();
            if (_influences.ContainsKey(type))
                _influences[type] = influence;
            else
                _influences.Add(type, influence);
        }
        public bool RemoveInfluence(IInfluence influence)
        {
            if (influence == null)
                throw new ArgumentNullException(nameof(influence));
            var type = influence.GetType();
            return _influences.Remove(type);
        }
        public bool Contains<T>() where T : IInfluence
        {
            return _influences.ContainsKey(typeof(T));
        }
        public T FindInfluence<T>() where T : IInfluence
        {
            var type = typeof(T);
            return _influences.ContainsKey(type) ? (T)_influences[type] : default;
        }
        public void Update()
        {
            foreach (var r in _influences.Values)
                if (r != null && r.Enabled)
                    r.Update();
        }
    }
}
