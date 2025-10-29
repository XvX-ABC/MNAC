using Tests.Characters.Humanoid;
using Tests.States;
using Tests.Utilities.Composable;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons
{
    public interface IArmedWeaponArmBehaviour : Behaviours.Arms.IArmedWeaponArmBehaviour, IComponent, IWithCallbackPlayableState<object>
    {
        //TODO: 设置身体部位
        public HumanPart Part { get; set; }
    }
}
