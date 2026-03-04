using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Characters.Animations;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Animations
{
    [Obsolete]
    internal class HumanAnimationDefinitionsLoader : MonoBehaviour, IHumanAnimationDefinitions
    {
        [SerializeField]
        HumanoidAnimationDefinitions_SO _definitions;

        public IHumanArmAnimationDefinitions LeftArmDefinitions => _definitions.LeftArmDefinitions;

        public IHumanArmAnimationDefinitions RightArmDefinitions => _definitions.RightArmDefinitions;

    }
}
