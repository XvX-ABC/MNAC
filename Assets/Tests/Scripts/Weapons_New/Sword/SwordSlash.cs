using Codice.Client.BaseCommands;
using Tests.Interaction;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    internal class SwordSlash : SwordBehaviourComponent
    {
        [SerializeField]
        float _damagePoint;
        protected override SwordActionType actionType => SwordActionType.Slash;
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            sword.HitAction += WhenHitObj;
        }
        public override void Dispose()
        {
            sword.HitAction -= WhenHitObj;
            base.Dispose();
        }
        void WhenHitObj(GameObject obj)
        {
            if (obj.TryGetComponent<IDamageable>(out var d))
            {
                d.HP.ReceivePoint(_damagePoint);
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
