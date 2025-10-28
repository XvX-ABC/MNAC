using Minimalist.Bar;
using Minimalist.Quantity;
using Tests.Interaction;
using Tests.Interaction.Influence;
using UnityEngine;
using HealthInfluence = Tests.Interaction.Influence.Health;
using StunInfluence = Tests.Interaction.Influence.Stun;
namespace Tests.Characters.Interaction
{
    [Interactable]
    public class Character_T0 : CharacterBase, ICharacter_T0
    {
        [SerializeField]
        QuantityBhv _healthQuantitySetter;
        Stun _stun;
        Health _hp;



        ICharacterDefinitions_T0 _definitions;
        InfluenceCore _influenceCore;
        public IStun Stun => _stun;
        public IHealth HP => _hp;

        public InfluenceCore InfluenceCore
        {
            get => _influenceCore;
            set
            {
                _influenceCore = value;
                if (_influenceCore != null)
                {
                    _stun.Influence = _influenceCore.FindInfluence<StunInfluence>() ?? throw new InfluenceNotExistInCoreException<StunInfluence>();
                    _hp.Influence = _influenceCore.FindInfluence<HealthInfluence>() ?? throw new InfluenceNotExistInCoreException<HealthInfluence>();
                }
                else
                {
                    _stun.Influence = null;
                    _hp.Influence = null;
                }
            }
        }

        void Awake()
        {
            _definitions = GetComponent<ICharacterDefinitions_T0>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ICharacterDefinitions_T0));
            var maxHP = _definitions.MaxHP;
            _hp = new(maxHP, new MinimalistHealthEffects(_healthQuantitySetter, maxHP));
            _stun = new();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            _hp.enabled = true;
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            _hp.ReceivePoint(-_hp.Point);
            _stun.EndEarly();
            _hp.enabled = false;
        }
    }
}
