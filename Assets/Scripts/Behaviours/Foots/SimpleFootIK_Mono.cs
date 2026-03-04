using RootMotion.FinalIK;
using UnityEngine;

namespace MNAC.Behaviours.Foots
{
    public class SimpleFootIK_Mono : MonoBehaviour
    {
        [SerializeField]
        LayerMask _layer;
        [SerializeField]
        Vector3 _positionOffset;
        [SerializeField]
        Vector3 _worldUpward;
        [SerializeField]
        LegIK _ik;
        public void LateUpdate()
        {
            var pos = this.transform.position;
            var ray = new Ray(pos, -_worldUpward);
            if ( Physics.Raycast(ray, out var hitInfo))
            {
                _ik.solver.IKPosition = _positionOffset + hitInfo.point;
                _ik.solver.IKRotation = Quaternion.FromToRotation(_worldUpward, hitInfo.normal) * this.transform.rotation;
            }
        }
    }
}
