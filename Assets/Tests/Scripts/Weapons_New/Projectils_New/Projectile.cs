using System;
using Tests.Weapons_New.Projectiles;
using UnityEngine;

namespace Tests.Weapons.Projectiles_New
{
    public abstract class Projectile : MonoBehaviour, IProjectile
    {
        Action<IProjectile> _actionStartCallback;
        Action<IProjectile> _actionEndCallback;

        public GameObject Obj => this.gameObject;

        public Action<IProjectile> ActionStartCallback { get => _actionStartCallback; set => _actionStartCallback = value; }
        public Action<IProjectile> ActionEndCallback { get => _actionEndCallback; set => _actionEndCallback = value; }
        public abstract Action<IProjectile, GameObject> HitAction { get; set; }

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
