using Tests.Interaction;
using Tests.Interaction.Influences;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    internal class SwordSlash : SwordBehaviourComponent
    {
        [SerializeField]
        float _damagePoint;
        [SerializeField]
        float _knockbackPower;
        protected override SwordActionType actionType => SwordActionType.Slash;
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            sword.HitAction += WhenHitTarget;
        }
        public override void Dispose()
        {
            sword.HitAction -= WhenHitTarget;
            base.Dispose();
        }
        void WhenHitTarget(GameObject obj)
        {
            if (obj.TryGetComponent<IDamageable>(out var d))
            {
                d.HP.ReceivePoint(_damagePoint);
            }
            if (obj.TryGetComponent<IKnockbackable>(out var k))
            {
                var originPos = sword.OriginPosition;
                var targetPos = obj.transform.position;
                var tpos = targetPos - originPos;
                tpos = Vector3.ProjectOnPlane(tpos, Vector3.up); // TODO：法线改为LocomotionCore上下文的地面法线
                k.Knockback.ReceiveForce(tpos.normalized * _knockbackPower);
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (sword != null)
                sword.tipTrigger.enabled = true;
        }
        protected override void OnDisable()
        {
            if (sword != null)
                sword.tipTrigger.enabled = false;
            base.OnDisable();
        }
    }
}
