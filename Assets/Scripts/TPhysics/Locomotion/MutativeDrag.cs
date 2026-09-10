using System;
using MNAC.Utilities.Timeline;
using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 在一段时间内把刚体 drag 从 Range.x 线性过渡到 Range.y。
    [Serializable]
    public class MutativeDrag : LocomotionModuleBase
    {
        [NonSerialized] ITimeline _timeline;
        [SerializeField] float _transitionalDuration;
        [NonSerialized] Rigidbody _rbody;
        [SerializeField] Vector2 _range;

        public MutativeDrag() : this(0f, Vector2.zero)
        {
        }

        public MutativeDrag(float transitionalDuration, Vector2 range)
        {
            TransitionalDuration = transitionalDuration;
            _timeline = new Timeline(_transitionalDuration);
            _range = range;
        }

        public float TransitionalDuration
        {
            get => _transitionalDuration;
            set => _transitionalDuration = Mathf.Max(0f, value);
        }

        public Vector2 Range
        {
            get => _range;
            set => _range = value;
        }

        public override Context OnStart(Context context)
        {
            _rbody = context.Rbody;
            _timeline ??= new Timeline(_transitionalDuration); // 反序列化后懒重建
            _timeline.Restart();
            return context;
        }

        public override Context OnUpdate(Context context)
        {
            _rbody.drag = Mathf.Lerp(_range.x, _range.y, _timeline.NormalizedTime);
            _timeline.OnUpdate(Time.deltaTime);
            return context;
        }

        public override Context OnEnd(Context context)
        {
            context.Rbody.drag = _range.x;
            return context;
        }
    }
}
