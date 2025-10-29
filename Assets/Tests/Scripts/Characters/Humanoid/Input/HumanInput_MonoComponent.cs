using UnityEngine;

namespace Tests.Characters.Humanoid.Interaction.Input
{
    public class HumanInput_MonoComponent : MonoBehaviour
    {
        [SerializeField]
        HumanInput _input;
        public static implicit operator HumanInput(HumanInput_MonoComponent mc)
        {
            return mc._input;
        }
    }
}
