using MNAC.Characters.Humanoid;
using MNAC.States;
using MNAC.Utilities.Composable;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms.Weapons
{
    public interface IArmedArmBehaviour : Behaviours.Arms.IArmedArmBehaviour, IComponent, IWithCallbackPlayableState<object>
    {
        public HumanBodyPart Part { get; set; }
    }
}
