using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons.Sword;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Weapons.Sword
{
    [RequireComponent(typeof(WeaponLoad))]
    public class SwordBase : MonoBehaviour, ISword
    {
        [SerializeField]
        float _slashRadius;
        [SerializeField]
        float _damagePoint;
        SwordTargetsTrigger _trigger;
        bool _enabledDamage;
        public string Name => this.name;

        public WeaponType Type => WeaponType.Sword;

        public GameObject Obj => this.gameObject;

        public float SlashRadius => _slashRadius;

        public bool EnableDamage
        {
            get => _enabledDamage;
            set
            {
                _trigger.enabled = value;
                _enabledDamage = value;
            }
        }

        void Awake()
        {
            _trigger = GetComponentInChildren<SwordTargetsTrigger>() ?? throw new ComponentCantFindException(this.gameObject, typeof(SwordTargetsTrigger));
            _trigger.WhenTargetEntryAction += WhenHitEnemy;
            _trigger.enabled = false;
        }
        void WhenHitEnemy(GameObjTarget target)
        {
            if (!_enabledDamage)
                return;
            var obj = target.Obj;
            if (obj == null)
                throw new NullReferenceException(nameof(target.Obj));
            var item = obj.GetComponent<IDamageable>();
            item.HP.ReceivePoint(-_damagePoint);
        }
        public void WhenMounted(GameObject mountPoint)
        {
            //throw new NotImplementedException();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void WhenUnmounted(GameObject mountPoint)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        private void OnDrawGizmosSelected()
        {
            var pos = this.transform.position;
            var tpos = pos + this.transform.forward * _slashRadius;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(pos, tpos);
        }

    }
}
