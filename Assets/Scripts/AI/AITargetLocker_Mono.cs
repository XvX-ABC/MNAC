using MNAC.AI;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace MNAC.AI
{
    internal class AITargetLocker_Mono : AIComponent_Mono
    {
        [SerializeField]
        internal AITargetLocker locker;
        [SerializeField]
        Vector2 _randomRadiusRange;
        Random _random;
        bool _didAwake;

        public Vector2 RandomRadiusRange { get => _randomRadiusRange; set => _randomRadiusRange = new Vector2(Mathf.Min(value.x), Mathf.Max(value.y)); }

        protected override void Awake()
        {
            base.Awake();
            _random = new((uint)GetInstanceID());
            RandomRadiusRange = _randomRadiusRange;
        }
        private void OnEnable()
        {
            if (locker != null)
            {
                locker.Enabled = true;
                locker.StartCoroutine(this);
            }
        }
        private void OnDisable()
        {
            if (locker != null)
            {
                locker.Enabled = false;
                locker.StopCoroutine();
            }
        }
        public override void Initialize(AIComponentContext context)
        {
            base.Initialize(context);
            locker.Initialize(context);
            SetRadius();
            if (this._didAwake && this.enabled)
            {
                locker.StartCoroutine(this);
                locker.Enabled = this;
            }
        }
        void SetRadius()
        {
            var r = _random.NextFloat(_randomRadiusRange.x, _randomRadiusRange.y);
            locker.CatchRadius = r;
        }
        public override void Dispose()
        {
            locker.Dispose();
            locker.StopCoroutine();
            base.Dispose();
        }
        private void FixedUpdate()
        {
            locker.OnFixedUpdate();
        }
        private void LateUpdate()
        {
            locker.OnLateUpdate();
        }

    }
}
