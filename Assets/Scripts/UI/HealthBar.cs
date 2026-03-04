using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Interaction;
using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.UI
{
    public class HealthBar : UIComponent, IHealthCallback
    {
        [SerializeField]
        protected ProgressSlider slider;

        public float TriggerProportion => 1;

        public bool RepetitiveExecution => true;

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

        public void Execute(GameObject obj, IHealth health)
        {
            var point = health.Point;
            var maxPoint = health.MaxPoint;
            var proportion = maxPoint > 0 ? point / maxPoint : 0;
            slider.Value = proportion;
        }
    }
}
