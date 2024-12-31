#define TESTS
using UnityEngine;
namespace Tests.Locomotion.Arms
{
    public class ShoulderControlBase : MonoBehaviour
    {
        [SerializeField]
        GameObject _body;
        [SerializeField]
        GameObject _shoulder_end;
        [SerializeField]
        GameObject _elbow;
        //GameObject _handle;
        Tests.ITarget _target;
        Vector3 _forward;
        Vector3 _up;

        private void Start()
        {
            var f = _elbow.transform.position - this.gameObject.transform.position;
            _forward = _elbow.transform.localRotation * f.normalized;


            _up = this.transform.up;
#if TESTS
            _target = GetComponent<Tests.ITarget>();
#endif
        }

        Quaternion CalculateHorizontalRotation()
        {
            var currentPos = _shoulder_end.transform.position;
            var targetPos = _target.Locomotion.Position;
            var tv_w = targetPos - currentPos;
            var tv_b = Vector3.ProjectOnPlane(tv_w, _body.transform.up);
            return Quaternion.LookRotation(tv_b, _up);
        }
        float a;
        Quaternion CalculateVerticalRotation(Quaternion preRotation)
        {
            var tv0 = _target.Locomotion.Position - _shoulder_end.transform.position;
            var tv1 = Vector3.ProjectOnPlane(Quaternion.Inverse(preRotation) * tv0, Vector3.up);
            var r = Quaternion.LookRotation(tv1, Vector3.up);
            return r;
        }
        private void Update()
        {
            var hrotaton = CalculateHorizontalRotation();
            var vrotation = CalculateVerticalRotation(hrotaton);
            this.transform.rotation = hrotaton * vrotation;
            Debug.DrawLine(this.transform.position, this.transform.position + this.transform.up, Color.magenta);
        }
        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            //var pos = this.transform.position;
            //var lengthToEnd = (pos - _shoulder_end.transform.position).magnitude;
            //Gizmos.color = Color.blue;
            //Gizmos.DrawLine(pos, pos + this.transform.forward * 13);
            //var p = pos + this.transform.up * lengthToEnd;
            //Gizmos.DrawLine(p, p + this.transform.forward * 13);
            //Gizmos.color = Color.green;
            //Gizmos.DrawLine(pos, pos + this.transform.up * 3);
            //Gizmos.color = Color.red;
            //Gizmos.DrawLine(pos, pos + this.transform.right * 3);
        }
    }
}