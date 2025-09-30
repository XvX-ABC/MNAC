using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Interaction.Influence
{
    public class InfluenceCore
    {
        Dictionary<Type, Influence> _influences;
        public InfluenceCore(params Influence[] influences)
        {
            this._influences = new();
            foreach (var i in influences)
            {
                AddInfluence(i);
            }
        }
        public void AddInfluence(Influence influence)
        {
            if (influence == null)
                throw new ArgumentNullException(nameof(influence));
            var type = influence.GetType();
            if (_influences.ContainsKey(type))
                _influences[type] = influence;
            else
                _influences.Add(type, influence);
        }
        public bool RemoveInfluence(Influence influence)
        {
            if (influence == null)
                throw new ArgumentNullException(nameof(influence));
            var type = influence.GetType();
            return _influences.Remove(type);
        }
        public bool Contains<T>() where T : Influence
        {
            return _influences.ContainsKey(typeof(T));
        }
        public T FindInfluence<T>() where T : Influence
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
