using System;
using System.Collections.Generic;
using Tests.Input;
using Tests.TPhysics;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters
{
    public interface ITargetsCatcher_Obsolete
    {
        public Blackboard Blackboard { get; set; }
        public IReadOnlyList<ITarget_Obsolete> Targets { get; }
        public Action<IList<ITarget_Obsolete>> TargetsChangedAction { get; set; }
        public void AddTarget(ITarget_Obsolete target);
        public void RemoveTarget(ITarget_Obsolete target);
    }
    public interface ILauncherTargetsCatcherDefinitions
    {
        LayerMask TerrainMask { get; }
        LayerMask TargetsMask { get; }
    }
    [Obsolete]
    public interface ITarget_Obsolete
    {
        public Vector3 Position { get; }
    }

    [Serializable]
    public class TargetsCatcher_Obsolete : ITargetsCatcher_Obsolete
    {
        Blackboard _blackboard;
        List<ITarget_Obsolete> _targets;
        Action<IList<ITarget_Obsolete>> _targetsChangedAction;
        bool _updated;
        [SerializeField]
        int count;
        public Blackboard Blackboard
        {
            get => _blackboard;
            set
            {
                _blackboard = value;
            }
        }
        public IList<ITarget_Obsolete> Targets { get => _targets; }
        public Action<IList<ITarget_Obsolete>> TargetsChangedAction { get => _targetsChangedAction; set => _targetsChangedAction = value; }

        IReadOnlyList<ITarget_Obsolete> ITargetsCatcher_Obsolete.Targets => _targets;
        public TargetsCatcher_Obsolete()
        {
            _targets = new();
        }

        public void OnAwake()
        {
        }
        public void OnUpdate()
        {
            //if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            //{
            //    if (_targets.Contains(_target))
            //        _targets.Remove(_target);
            //    else
            //        _targets.Add(_target);
            //    _targetsChangedAction?.Invoke(_targets);
            //}
            if (_updated)
            {
                _updated = false;
                _targetsChangedAction?.Invoke(_targets);
            }
        }

        void ITargetsCatcher_Obsolete.AddTarget(ITarget_Obsolete target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            this._targets.Add(target);
            count++;
            _updated = true;
        }

        void ITargetsCatcher_Obsolete.RemoveTarget(ITarget_Obsolete target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            this._targets.Remove(target);
            count--;
            _updated = true;
        }
    }
}
