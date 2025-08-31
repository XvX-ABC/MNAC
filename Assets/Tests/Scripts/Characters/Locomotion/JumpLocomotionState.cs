using Locomotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.TPhysics.Locomotion;

namespace Tests.Characters.Locomotion
{
    internal class JumpLocomotionState : LocomotionStateBase
    {
        JumpLocomotion _locomotion;
        public JumpLocomotionState(IJumpDefinitions definitions, bool enabled = true) : base("jump", 0, enabled)
        {
            _locomotion = new(definitions);
        }

        protected override ILocomotionModule module => _locomotion;
    }
}
