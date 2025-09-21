using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Interaction.Influence;
using TMPro.EditorUtilities;

namespace Tests.Characters.Interaction
{
    internal class InfluenceReceivingCoreComponent : ComponentNode<InfluenceReceivingCore>
    {
        InfluenceReceivingCore _core;
        public override string Name => "influense_receiving_core";

        internal override InfluenceReceivingCore component => _core;
        public InfluenceReceivingCoreComponent()
        {
            var stunReceptor = new StunReceptor();
            _core = new InfluenceReceivingCore(stunReceptor);
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Influence_Receiving_Core, _core);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Influence_Receiving_Core);
        }
    }

}
