using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    internal class SwordSlashEffector : SwordEffector
    {
        [SerializeField]
        LayerMask _layerMask;
        [SerializeField]
        SwordHitEffect _hitEffect;

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
    
        protected override void WhenTargetEnter(GameObject ob)
        {
        }

        protected override void WhenTargetExit(GameObject obj)
        {
            var pos = sword.OwnerObj?.transform?.position ?? sword.transform.position;
            var tpos = obj.transform.position;

            var direction = Vector3.ProjectOnPlane(tpos - pos, sword.worldUp);
            var ray = new Ray(pos, direction);
            if (Physics.Raycast(ray, out var hitInfo))
            {
                var point = hitInfo.point;
                Debug.DrawLine(pos, point, Color.red, 10);

                _hitEffect.transform.position = point;
                _hitEffect.transform.SetParent(null);
                _hitEffect.Play();
            }


        }
    }
}
