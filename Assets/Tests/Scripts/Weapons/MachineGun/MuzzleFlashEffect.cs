using System;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    [RequireComponent(typeof(Animator))]
    public class MuzzleFlashEffect : MonoBehaviour
    {
        Animator _animator;
        [SerializeField]
        string _triggerName;
        [SerializeField, Range(0f, 10f)]
        float _duration;
        [SerializeField]
        string _speedMultiplierName;
        [SerializeField]
        float _clipLength;
        public float Duration=> _duration;
        void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        private void Start()
        {
            var m = _clipLength / _duration;
            _animator.SetFloat(_speedMultiplierName, m);
        }
        public void Play()
        {
            _animator.SetTrigger(_triggerName);
        }

    }
}
