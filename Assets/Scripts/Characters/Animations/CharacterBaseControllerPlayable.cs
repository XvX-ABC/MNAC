using MNAC.Animations;
using UnityEngine;
using UnityEngine.Playables;

namespace MNAC.Characters
{
    internal class CharacterBaseControllerPlayable : ControllerPlayable
    {
        public CharacterBaseControllerPlayable(PlayableGraph graph, Animator animator) : base(graph, animator)
        {
        }

        public CharacterBaseControllerPlayable(PlayableGraph graph, RuntimeAnimatorController controller) : base(graph, controller)
        {
        }
    }
}
