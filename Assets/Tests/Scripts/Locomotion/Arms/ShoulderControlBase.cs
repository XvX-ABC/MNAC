#define TESTS
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UInput = UnityEngine.Input;
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
        [SerializeField]
        GameObject _hand;
        //GameObject _handle;
        Tests.ITarget _target;
        Vector3 _localForward;
        Quaternion _r;
        Vector3 _up;

        bool _applyRotation;
        Quaternion _rotation;

        private void Start()
        {
            //var f = _elbow.transform.position - _shoulder_end.gameObject.transform.position;
            //_forward = _elbow.transform.localRotation * f.normalized;

            UpdateForward();

            _up = this.transform.up;
#if TESTS
            _target = GetComponent<Tests.ITarget>();

            _rotation = this.transform.rotation;
#endif
        }
        void UpdateForward()
        {
            var f = _elbow.transform.position - _shoulder_end.transform.position;
            var r = Quaternion.FromToRotation(f, _elbow.transform.up);
            f = Quaternion.Inverse(this.transform.rotation) * r * f;
            _localForward = f;
            _r = r;
        }
        Quaternion CalculateHorizontalRotation()
        {
            var currentPos = _shoulder_end.transform.position;
            var targetPos = _target.Locomotion.Position;
            var tv_w = targetPos - currentPos;
            var tv_b = Vector3.ProjectOnPlane(tv_w, _body.transform.up);
            return Quaternion.LookRotation(tv_b, _up);
        }
        Quaternion CalculateVertcalRotation_0(Quaternion preRotation)
        {
            var rotation = Quaternion.Inverse(preRotation);
            var tv0 = (_target.Locomotion.Position - _elbow.transform.position).normalized;
            var tv1 = Vector3.ProjectOnPlane(rotation * tv0, Vector3.up);

            var forward_0 = Vector3.ProjectOnPlane(_localForward, Vector3.up);

            var r = Quaternion.FromToRotation(forward_0, tv1);

            return r;
        }

        private void Update()
        {
            if (UInput.GetKeyDown(KeyCode.Space))
                _applyRotation = !_applyRotation;

            var hrotaton = CalculateHorizontalRotation();
            var vrotation = CalculateVertcalRotation_0(hrotaton);
            if (_applyRotation)
                this.transform.rotation = hrotaton * vrotation;


            //this.transform.rotation = hrotaton * vrotation;
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