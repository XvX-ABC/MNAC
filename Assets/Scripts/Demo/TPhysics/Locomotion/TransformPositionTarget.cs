using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Demo.TPhysics.Locomotion
{
    /// <summary>
    /// 把场景里的 Transform 适配成 <see cref="IPositionTarget"/>，
    /// 供 RotationByMouseOrTargetLocomotion 在测试场景里直接引用。
    /// </summary>
    public sealed class TransformPositionTarget : MonoBehaviour, IPositionTarget
    {
        public Vector3 Position => transform.position;
    }
}
