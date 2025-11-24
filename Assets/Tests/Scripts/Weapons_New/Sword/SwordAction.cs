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
        internal SwordAction(Sword owner, SwordActionType type)
        {
            _owner = owner;
            _owner.tipTrigger.enabled = _enabled;
            _components = new();
            _type = type;
        }
        public virtual bool Enabled
        {
            get => _enabled;
            set
            {
                foreach (var comp in _components)
                    comp.enabled = value;
                _enabled = value;
                if (value)
                    _owner.currentEnabledActions.Add(this);
                else
                    _owner.currentEnabledActions.Remove(this);
            }
        }

        public SwordActionType Type { get => _type; set => _type = value; }
        internal bool Contains(SwordActionComponent comp)
        {
            return _components.Contains(comp);
        }
        internal void AddComponent(SwordActionComponent comp)
        {
            if (comp == null)
                throw new ArgumentNullException(nameof(comp));
            comp.enabled = _enabled;
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