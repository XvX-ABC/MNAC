using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.UI
{
    [RequireComponent(typeof(TargetsDisplay))]
    public class TargetsDisplay_Composable : UIComponent
    {
        TargetsDisplay _display;
        public Vector3 TargetWorldPos
        {
            get => _display.TargetWorldPos;
            set => _display.TargetWorldPos = value;
        }
        public bool Activated
        {
            get => _display.Activated;
            set => _display.Activated = value;
        }
        protected override void Awake()
        {
            base.Awake();
            _display = GetComponent<TargetsDisplay>();
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValue<Camera>(UIBlackboardFields.Camera_Main, out var camera);
            _display.Camera = camera;
            blackboard.TryRegisterField(UIBlackboardFields.Targets_Display, _display);
        }
    }
}
