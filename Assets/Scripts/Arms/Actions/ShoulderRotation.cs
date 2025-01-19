using UnityEngine;

namespace Assets.Scripts.Arms.Actions
{
    internal class ShoulderRotation : MonoBehaviour, IArmLocomotionComponent
    {
        GameObject _shoulder_end;
        GameObject _elbow;
        GameObject _hand;
        GameObject _body;
        IArmTarget _target;
        Vector3 _up;

        internal GameObject ShoulderEnd { get => _shoulder_end; set => _shoulder_end = value; }
        internal GameObject Elbow { get => _elbow; set => _elbow = value; }
        internal GameObject Hand { get => _hand; set => _hand = value; }
        public GameObject Body { get => _body; set => _body = value; }
        public IArmTarget Target { get => _target; set => _target = value; }
        void Awake()
        {
            _up = this.transform.up;
        }
        Quaternion CalculateHorizontalRotation()
        {
            var currentPos = _shoulder_end.transform.position;
            var targetPos = _target.Position;
            var tv_w = targetPos - currentPos;
            var tv_b = Vector3.ProjectOnPlane(tv_w, _body.transform.up);
            //this.transform.rotation = Quaternion.LookRotation(tv_b, _up);
            return Quaternion.LookRotation(tv_b, _up);

        }
        Quaternion CalculateVerticalRotation(Quaternion rotation)
        {
            var u = rotation * Vector3.up;

            var currentPos = _shoulder_end.transform.position;
            var targetPos = _target.Position;
            var tv_w = targetPos - currentPos;
            var tv_b = Vector3.ProjectOnPlane(tv_w, u);
            var tv_f = Quaternion.Inverse(rotation) * tv_b;
            Debug.DrawLine(currentPos, currentPos + tv_f.normalized * 3, Color.magenta);

            return Quaternion.LookRotation(tv_f, Vector3.up);
        }
        void Update()
        {
            var hrotation = CalculateHorizontalRotation();
            var vrotation = CalculateVerticalRotation(hrotation);
            this.transform.rotation = hrotation * vrotation;
            //this.transform.rotation = CalculateHorizontalRotation() * Quaternion.Euler(0, 10 * Time.time, 0);
        }
        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            var pos = this.transform.position;
            var lengthToEnd = (pos - _shoulder_end.transform.position).magnitude;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos, pos + this.transform.forward * 13);
            var p = pos + this.transform.up * lengthToEnd;
            Gizmos.DrawLine(p, p + this.transform.forward * 13);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + this.transform.up * 3);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + this.transform.right * 3);
        }
    }
}
