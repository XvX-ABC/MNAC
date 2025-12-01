using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Characters.Animations;
using UnityEngine;

namespace Tests.Characters.Humanoid.Animations
{
    internal class HumanAnimationDefinitionsLoader : MonoBehaviour, IHumanAnimationDefinitions
    {
        [SerializeField]
        HumanAnimationDefinitions_SO _definitions;

        public IHumanArmAnimationDefinitions LeftArmDefinitions => _definitions.LeftArmDefinitions;

        public IHumanArmAnimationDefinitions RightArmDefinitions => _definitions.RightArmDefinitions;

        public IStunningAnimationDefinitions Stunning => _definitions.Stunning;

        public IDeathAnimationDefinitions Death => _definitions.Death;
    }
}
