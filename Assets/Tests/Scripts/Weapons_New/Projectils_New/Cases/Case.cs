using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons.Projectiles_New;
using UnityEngine;

namespace Tests.Weapons_New.Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    internal class Case : Projectile
    {
        [SerializeField]
        [Range(0, 120)]
        float _survivalDuration;
        float _timer;
        Rigidbody _rb;
        Action<Case> _survivalDurationEndAction;
        public Action<Case> SurvivalDurationEndAction { get => _survivalDurationEndAction; set => _survivalDurationEndAction = value; }
        public override Action<IProjectile, GameObject> HitAction { get; set; }
        public Rigidbody Rbody { get => _rb; }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        void OnEnable()
        {
            _timer = 0;
        }
        private void OnCollisionEnter(Collision collision)
        {
            _rb.velocity = Vector3.zero;
        }
        private void OnCollisionStay(Collision collision)
        {
            _rb.velocity = Vector3.zero;
        }
        void Update()
        {
            if (_timer >= _survivalDuration)
            {
                _survivalDurationEndAction?.Invoke(this);
                EndAction();
                _timer = 0;
            }
            _timer += Time.deltaTime;
        }
    }
}
