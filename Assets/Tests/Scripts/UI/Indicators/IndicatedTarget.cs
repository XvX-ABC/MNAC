using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.UI
{
    public abstract class IndicatedTarget : ComponentBase_MonoComponent, IIndicatedTarget
    {
        [SerializeField]
        protected IndicatorType indicatorType;
        protected Indicator indicator;
        [SerializeField]
        IndicatorsManager _lockManager;

        public bool IsValid { get => this.enabled; }
        public IndicatorType IndicatorType { get => indicatorType; }
        public virtual Indicator Indicator { get => indicator; set => indicator = value; }
        public IndicatorsManager Manager
        {
            get => _lockManager;
            set
            {
                if (_lockManager)
                {
                    _lockManager.RemoveTarget(this);
                }
                if (value)
                {
                    value.AddTarget(this);
                }

                _lockManager = value;
            }
        }

        protected virtual void OnEnable()
        {
            //TargetsLockManager.RegisterLocker(this);
            _lockManager?.AddTarget(this);
        }

        protected virtual void OnDisable()
        {
            //TargetsLockManager.UnregisterLocker(this);
            _lockManager?.RemoveTarget(this);
        }

        private void OnDestroy()
        {
            _lockManager?.RemoveTarget(this);
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<IndicatorsManager>(UIBlackboardFields.Indicators_Manager, out _lockManager);

            Manager = _lockManager;

        }

        public virtual Vector3 GetScreenPosition(Camera camera)
        {
            return camera.WorldToScreenPoint(this.transform.position);
        }
    }
}
