using Tests.Behaviours.Arms.Weapons.Sword;
using Tests.Interaction.Targets;
using UnityEngine;

namespace Tests.Weapons.Sword
{
    public class SwordTrigger : TriggerTargetsCatcher
    {
    }
    [RequireComponent(typeof(WeaponLoad))]
    public class SwordBase : MonoBehaviour, ISword
    {
        [SerializeField]
        float _slashRadius;

        public string Name => this.name;

        public WeaponType Type => WeaponType.Sword;

        public GameObject Obj => this.gameObject;

        public float SlashRadius => _slashRadius;

        void Awake()
        {
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
