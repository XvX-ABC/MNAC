using Tests.Utilities.Composable;
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
            if (!enabled)
                return;
            var obj = collider.gameObject;
            AddItemImpl(obj);
        }
        public override void OnTriggerExit(Collider collider)
        {
            if (!enabled)
                return;
            var obj = collider.gameObject;
            RemoveItemImpl(obj);
        }
    }
}
