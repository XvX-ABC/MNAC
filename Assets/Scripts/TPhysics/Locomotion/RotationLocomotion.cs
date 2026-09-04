using UnityEngine;

namespace MNAC.TPhysics.Locomotion
{
    /// 朝目标点平滑转向的底层算法（内部使用，由上层装饰器填充 Origin/TargetPos）。
    internal class RotationLocomotion : LocomotionModuleBase
    {
        const float TurnSpeed = 15f;
        Vector3 _origin;
        Vector3 _forward = Vector3.forward;
        Vector3 _targetPos;
        Quaternion _offset = Quaternion.identity;

        public Vector3 Origin { get => _origin; set => _origin = value; }
        public Vector3 Forward { get => _forward; set => _forward = value; }
        public Vector3 TargetPos { get => _targetPos; set => _targetPos = value; }
        public Quaternion Offset { get => _offset; set => _offset = value; }

        public override Context OnUpdate(Context context)
        {
            context.CurrentRotation = Quaternion.Slerp(context.CurrentRotation, CalculateRotation(), Time.deltaTime * TurnSpeed);
            return context;
        }

        Quaternion CalculateRotation()
        {
            var delta = _targetPos - _origin;
            return _offset * Quaternion.FromToRotation(_forward, delta.normalized);
        }
    }
}
