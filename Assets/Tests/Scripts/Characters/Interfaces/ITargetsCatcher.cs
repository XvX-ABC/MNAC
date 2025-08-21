using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Input;
using UnityEngine;

namespace Tests.Characters
{
    public interface ITargetsCatcher
    {
        public Blackboard Blackboard { get; set; }
        public IReadOnlyList<ITarget> Targets { get; }
        public Action<IList<ITarget>> TargetsChangedAction { get; set; }
        public void AddTarget(ITarget target);
        public void RemoveTarget(ITarget target);
    }
    [Serializable]
    public class TargetsCatcher_Debug : ITargetsCatcher
    {
        Blackboard _blackboard;
        List<ITarget> _targets;
        Action<IList<ITarget>> _targetsChangedAction;
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
        public IList<ITarget> Targets { get => _targets; }
        public Action<IList<ITarget>> TargetsChangedAction { get => _targetsChangedAction; set => _targetsChangedAction = value; }

        IReadOnlyList<ITarget> ITargetsCatcher.Targets => _targets;
        public TargetsCatcher_Debug()
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
            if(_updated)
            {
                _updated = false;
                    _targetsChangedAction?.Invoke(_targets);
            }
        }

        void ITargetsCatcher.AddTarget(ITarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            this._targets.Add(target);
            count++;
            _updated = true;
        }

        void ITargetsCatcher.RemoveTarget(ITarget target)
        {
            if(target==null)
                throw new ArgumentNullException(nameof(target));
            this._targets.Remove(target);
            count--;
          _updated=true;
        }
    }
}
