using MNAC.Utilities;
using UnityEngine;

namespace MNAC.Weapons.Sword
{
    internal class SwordSlashEffector : SwordEffector
    {
        [SerializeField]
        LayerMask _layerMask;
        [SerializeField]
        SwordHitEffect _hitEffect;
        [SerializeField]
        bool _drawLine_Debug;

        protected override SwordActionType actionType => SwordActionType.Slash;
        protected override void Awake()
        {
            base.Awake();
            _hitEffect.Parent = this.transform;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void WhenHitTarget(GameObject obj)
        {
            var pos = sword.transform.position;
            if (sword.OwnerObj != null)
                pos = sword.OwnerObj.transform.position;

            var tpos = obj.transform.position;
            if (_drawLine_Debug)
                Debug.DrawLine(pos, tpos, Color.red, 3);

            var direction = Vector3.ProjectOnPlane(tpos - pos, sword.worldUp);
            var ray = new Ray(pos, direction);
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _layerMask))
            {
                var point = hitInfo.point;

                _hitEffect.transform.position = point;
                _hitEffect.transform.SetParent(null);
                _hitEffect.Play();
            }
        }
    }
}
