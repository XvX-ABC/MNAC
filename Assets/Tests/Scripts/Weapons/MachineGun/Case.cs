using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    public interface IEjectable : IProjectile
    {
        public Action<IEjectable, GameObject> SurvivalDurationEndAction { get; set; }
    }
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Case : MonoBehaviour, IEjectable
    {
        [SerializeField, Range(0, 60)]
        float _survivalDuration;
        float _timer;
        Rigidbody _rb;
        Action<IEjectable, GameObject> _survivalDurationEndAction;
        public bool Enabled { get => this.enabled; set => this.enabled = value; }
        public Action<IProjectile, GameObject> HitAction { get => null; set { } }
        public Action<IEjectable, GameObject> SurvivalDurationEndAction { get => _survivalDurationEndAction; set => _survivalDurationEndAction = value; }

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
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
                _survivalDurationEndAction?.Invoke(this, this.gameObject);
                _timer = 0;
            }
            _timer += Time.deltaTime;
        }
    }
}