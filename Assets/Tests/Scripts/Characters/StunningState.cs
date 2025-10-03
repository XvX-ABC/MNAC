using System;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Characters
{
    internal class StunningState : CharacterBehaviourStateBase
    {
        public StunningState(ITimeline timeline, bool enabled = true) : base("stunning", 0, enabled)
        {
            base.timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
        }
    }
    internal class DiedState : CharacterBehaviourStateBase
    {
        GameObject _obj;
        Action<GameObject> _action;
        public DiedState(GameObject obj, Action<GameObject> deathAction, float duration = 0, bool enabled = true) : base("died", duration, enabled)
        {
            _obj = obj;
            _action = deathAction;
            this.timeline.EndAction += DoDied;
        }
        void DoDied(TimelineContext _)
        {
            if (_obj != null)
                _action?.Invoke(_obj);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            timeline.Restart();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            timeline.OnUpdate(Time.deltaTime);
        }
        public override void OnExit()
        {
            timeline.End();
            base.OnExit();
        }
    }
}
