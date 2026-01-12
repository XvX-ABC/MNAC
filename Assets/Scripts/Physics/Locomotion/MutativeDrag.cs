using Tests.Utilities.Timeline;
using UnityEngine;

namespace Tests.TPhysics.Locomotion
{
    public class MutativeDrag : LocomotionModuleBase
    {
        float _transitionalDuration;
        ITimeline _timeline;
        Rigidbody _rbody;
        Vector2 _range;
        public MutativeDrag(float transitionalDuration, Vector2 range)
        {
            TransitionalDuration = transitionalDuration;
            _timeline = new Timeline(_transitionalDuration);
            _range = range;
        }

        public float TransitionalDuration { get => _transitionalDuration; set => _transitionalDuration = Mathf.Max(0, value); }
        public Vector2 Range { get => _range; set => _range = value; }
        public override Context OnEnd(Context context)
        {
            context.Rbody.drag = _range.x;
            return context;
        }

        public override Context OnStart(Context context)
        {
            _rbody = context.Rbody;
            _timeline.Restart();
            return context;
        }

        public override Context OnUpdate(Context context)
        {
            var t = _timeline.NormalizedTime;
            UpdateDrag(t);
            _timeline.OnUpdate(Time.deltaTime);
            return context;
        }
        void UpdateDrag(TimelineContext ctx)
        {
            var t = ctx.NormalizedTime;
            var m = Mathf.Lerp(_range.x, _range.y, t);
            _rbody.drag = m;
        }
        void UpdateDrag(float t)
        {
            var m = Mathf.Lerp(_range.x, _range.y, t);
            _rbody.drag = m;
        }
    }
}
