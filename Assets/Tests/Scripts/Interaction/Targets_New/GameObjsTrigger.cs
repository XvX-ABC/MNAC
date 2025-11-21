using UnityEngine;

namespace Tests.Interaction
{
    public class GameObjsTrigger : TargetsTriggerBase<GameObject>
    {
        public GameObjsTrigger(Collider collider) : base(collider)
        {
        }

        public override void OnTriggerEnter(Collider collider)
        {
            var obj = collider.gameObject;
            AddItemImpl(obj);
        }

        public override void OnTriggerExit(Collider collider)
        {
            var obj = collider.gameObject;
            RemoveItemImpl(obj);
        }
    }
}
