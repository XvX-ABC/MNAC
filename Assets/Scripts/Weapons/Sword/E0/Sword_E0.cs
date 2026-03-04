using UnityEngine;

namespace MNAC.Weapons.Sword
{
    internal class Sword_E0 : Sword
    {
        [SerializeField]
        SwordExtensionControl _extensionControl;
        protected override void InitializeActions()
        {
            base.InitializeActions();
            var action = GetSwordAction(SwordActionType.Slash);
        }
    }
    internal class SwordSlashEffector_E0 : SwordSlashEffector
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
        }
    }
}
