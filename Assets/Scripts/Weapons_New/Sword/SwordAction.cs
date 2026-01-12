using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Tests.Weapons_New.Sword
{
    public class SwordAction
    {
        bool _enabled;
        List<SwordActionComponent> _components;
        Sword _owner;
        SwordActionType _type;
        [Obsolete]
        float _multiplier;
        float _duration;
        internal SwordAction(Sword owner, SwordActionType type, float multiplier = 1)
        {
            _owner = owner;
            _owner.tipTrigger.enabled = _enabled;
            _components = new();
            _type = type;
            _multiplier = 1;
        }
        public virtual bool Enabled
        {
            get => _enabled;
            set
            {
                foreach (var comp in _components)
                    comp.Enabled = value;
                _enabled = value;
                if (value)
                    _owner.currentEnabledActions.Add(this);
                else
                    _owner.currentEnabledActions.Remove(this);
            }
        }

        public SwordActionType Type { get => _type; set => _type = value; }
        [Obsolete]
        public float Multiplier
        {
            get => _multiplier;
            set
            {
                _multiplier = value;
                foreach (var c in _components)
                    c.multiplier = _multiplier;
            }
        }

        public float Duration
        {
            get => _duration;
            set
            {
                foreach (var c in _components)
                    c.duration = value;
                _duration = value;
            }
        }

        internal bool Contains(SwordActionComponent comp)
        {
            return _components.Contains(comp);
        }
        void SynchronizeTo(SwordActionComponent comp)
        {
            comp.enabled = _enabled;
            comp.multiplier = _multiplier;
            comp.duration = _duration;
        }
        internal void AddComponent(SwordActionComponent comp)
        {
            if (comp == null)
                throw new ArgumentNullException(nameof(comp));
            SynchronizeTo(comp);
            _components.Add(comp);
        }
        internal void RemoveComponent(SwordActionComponent comp)
        {
            if (comp == null)
                throw new ArgumentNullException(nameof(comp));
            _components.Remove(comp);
        }
    }
}