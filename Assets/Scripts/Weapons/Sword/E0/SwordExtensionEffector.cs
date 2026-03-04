using MNAC.Utilities.Blackboards;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace MNAC.Weapons.Sword
{
    internal class SwordExtensionEffector : SwordEffector
    {
        [SerializeField]
        Animator _animator;
        [SerializeField]
        string _clipName;
        [SerializeField]
        string _multiplier;
        [SerializeField]
        float _clipLength;
        [SerializeField]
        float _duration;

        public float Duration
        {
            get => _duration;
            set
            {
                _duration = Mathf.Max(0, value);
                UpdateMultiplier(_duration);
            }
        }
        protected override SwordActionType actionType => SwordActionType.Extension;
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            UpdateMultiplier(0);
            Play(0);
        }
        void UpdateMultiplier(float duration)
        {
            var m = duration == 0 ? 0 : _clipLength / duration;
            _animator.SetFloat(_multiplier, m * multiplier);

        }

        protected override void Awake()
        {
            base.Awake();
            UpdateMultiplier(0);
            Play(0);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateMultiplier(_duration);
            Play();
        }
        protected override void OnDisable()
        {
            UpdateMultiplier(-_duration);
            Play(1);
            base.OnDisable();
        }
        public void Play(ushort normalizeTime = 0)
        {
            _animator.Play(_clipName, 0, Mathf.Clamp01(normalizeTime));
        }

        protected override void WhenTargetEnter(GameObject ob)
        {
        }
        protected override void WhenTargetExit(GameObject obj)
        {
        }
    }
}
