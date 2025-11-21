using System;
using Tests.Utilities.Blackboards;
using Tests.Weapons_New.Projectiles;
using UnityEngine;

namespace Tests.Weapons.Projectiles_New
{
    public abstract class Projectile : MonoBehaviour, IProjectile
    {
        [SerializeField]
        ProjectileComponent[] _subComponents;
        Action<IProjectile> _actionStartCallback;
        Action<IProjectile> _actionEndCallback;
        protected Blackboard blackboard;

        public GameObject Obj => this.gameObject;

        public Action<IProjectile> ActionStartCallback { get => _actionStartCallback; set => _actionStartCallback = value; }
        public Action<IProjectile> ActionEndCallback { get => _actionEndCallback; set => _actionEndCallback = value; }
        public abstract Action<IProjectile, GameObject> HitAction { get; set; }
        protected virtual void Awake()
        {
            blackboard = new();
        }
        protected virtual void OnEnable()
        {
            foreach (var comp in _subComponents)
                comp.Initialize(blackboard);
        }
        protected virtual void OnDisable()
        {
            foreach (var comp in _subComponents)
                comp.Dispose();
        }
        public virtual void EndAction()
        {
            this.enabled = false;
            _actionEndCallback?.Invoke(this);
        }

        public virtual void StartAction()
        {
            this.enabled = true;
            _actionStartCallback?.Invoke(this);
        }
    }
}
