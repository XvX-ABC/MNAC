using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Utilities.Blackboards;

namespace Tests.Weapons_New.Sword
{
    internal class SwordSlash : SwordBehaviourComponent
    {
        protected override SwordActionType actionType => SwordActionType.Slash;
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
