using System;
using Tests.Interaction.Influence;
using StunInfluence = Tests.Interaction.Influence.Stun;
using HealthInfluence = Tests.Interaction.Influence.Health;
using UnityEngine;
namespace Tests.Characters.Interaction
{
    public class Character_T0 : CharacterBase, ICharacter_T0
    {
        Stun _stun;
        Health _hp;


        ICharacterDefinitions_T0 _definitions;
        CharacterCore _core;
        public IStun Stun => _stun;
        public IHealth HP => _hp;
        void Awake()
        {
            _definitions = GetComponent<ICharacterDefinitions_T0>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ICharacterDefinitions_T0));
            _core = GetComponent<CharacterCore>() ?? throw new ComponentCantFindException(this.gameObject, typeof(CharacterCore));
        }
        void OnEnable()
        {
            var influenceCore = _core.influenceCore ?? throw new NullReferenceException("characterCore.influenceCore");
            var stunInfluence = influenceCore.FindInfluence<StunInfluence>() ?? throw new InfluenceNotExistInCoreException<StunInfluence>();
            var healthInfluence = influenceCore.FindInfluence<HealthInfluence>() ?? throw new InfluenceNotExistInCoreException<HealthInfluence>();
            _hp = new(_definitions.MaxHP, healthInfluence);
            _stun = new(stunInfluence);
            healthInfluence.Enabled = true;
        }
        void OnDisable()
        {
            _hp.ReceivePoint(-_hp.Point);
            _stun.EndEarly();
            _hp.influence.Enabled = false;
        }
        void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"hp : {_hp.MaxPoint}, {_hp.Point}");
            GUILayout.EndVertical();
        }
    }
}
