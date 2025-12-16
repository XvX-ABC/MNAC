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
        IndicatorsManager _indicatorsManager;

        public bool IsValid { get => this.enabled; }
        public IndicatorType IndicatorType { get => indicatorType; }
        public virtual Indicator Indicator { get => indicator; set => indicator = value; }
        public IndicatorsManager Manager
        {
            get => _indicatorsManager;
            set
            {
                if (_indicatorsManager)
                {
                    _indicatorsManager.RemoveTarget(this);
                }
                if (value)
                {
                    value.AddTarget(this);
                }

                _indicatorsManager = value;
            }
        }

        protected virtual void OnEnable()
        {
            //TargetsLockManager.RegisterLocker(this);
            _indicatorsManager?.AddTarget(this);
        }

        protected virtual void OnDisable()
        {
            //TargetsLockManager.UnregisterLocker(this);
            _indicatorsManager?.RemoveTarget(this);
        }

        private void OnDestroy()
        {
            _indicatorsManager?.RemoveTarget(this);
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<IndicatorsManager>(UIBlackboardFields.Indicators_Manager, out _indicatorsManager);

            Manager = _indicatorsManager;

        }

        public virtual Vector3 GetScreenPosition(Camera camera)
        {
            return camera.WorldToScreenPoint(this.transform.position);
        }
    }
}
