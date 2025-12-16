using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.UI.Assets.Tests.Scripts.UI
{
    internal class HealthBar : UIComponent
    {
        [SerializeField]
        ProgressSlider _slider;
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(UIBlackboardFields.Health_Bar, this);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(UIBlackboardFields.Health_Bar);
            base.Dispose();
        }
    }
}
