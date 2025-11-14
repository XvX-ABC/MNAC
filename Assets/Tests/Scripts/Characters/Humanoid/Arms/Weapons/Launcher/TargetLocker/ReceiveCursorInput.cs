using System;
using System.Collections.Generic;
using Tests.Utilities.Timeline;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    internal class ReceiveCursorInput : TargetLockerState
    {
        List<Vector3> _posList;
        public ReceiveCursorInput(TargetLocker locker, string name, float duration = 0, bool enabled = true) : base(locker, name, duration, enabled)
        {
            _posList = new();
        }
        protected override ITimeline NewTimeline(float duration)
        {
            return new Timeline_V1(duration);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            _posList.Clear();
            timeline.Restart();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            _posList.Add(locker.CursorPositionDelta);
            timeline.OnUpdate(Time.deltaTime);
        }
        public override void OnExit()
        {
            base.OnExit();
            timeline.End();
            var pos = Vector3.zero;
            for (int i = 0; i < _posList.Count; i++)
            {
                pos += _posList[i];
            }
            locker.cursorPositionDeltaCache = pos / _posList.Count;
        }
        protected override void OnExecute(List<GameObject> caughtObjs)
        {
            throw new NotImplementedException();
        }
    }
}
