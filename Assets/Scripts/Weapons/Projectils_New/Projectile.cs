using System;
using MNAC.Interaction;
using MNAC.Utilities.Blackboards;
using MNAC.Weapons.Projectiles;
using UnityEngine;
using static MNAC.Weapons.Projectiles.IProjectile;

namespace MNAC.Weapons.Projectiles
{
    public abstract class Projectile : MonoBehaviour, IProjectile
    {

        [SerializeField]
        internal ProjectileComponent[] subComponents;
        Action<IProjectile> _actionStartCallback;
        Action<IProjectile> _actionEndCallback;
        protected Blackboard blackboard;
        [SerializeField]
        protected LayerMask layerMaskToHit;
        [SerializeField]
        protected TeamMask ownerTeamMask;

        public GameObject Obj => this.gameObject;

        public Action<IProjectile> ActionStartCallback { get => _actionStartCallback; set => _actionStartCallback = value; }
        public Action<IProjectile> ActionEndCallback { get => _actionEndCallback; set => _actionEndCallback = value; }
        public LayerMask LayerMaskToHit
        {
            get => layerMaskToHit;
            set
            {
                blackboard.TryRegisterFieldOrWriteValue(ProjectileFields.Hit_LayerMask, value);
                layerMaskToHit = value;
            }
        }

        public TeamMask TeamMask
        {
            get => ownerTeamMask;
            set
            {
                blackboard.TryRegisterFieldOrWriteValue(ProjectileFields.TeamMask, value);
                ownerTeamMask = value;
            }
        }

        protected virtual void Awake()
        {
            blackboard = new();
            LayerMaskToHit = layerMaskToHit;
        }
        protected virtual void OnEnable()
        {
            foreach (var comp in subComponents)
                comp.Initialize(blackboard);
        }
        protected virtual void Start() { }
        protected virtual void OnDisable()
        {
            foreach (var comp in subComponents)
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
