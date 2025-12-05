using System;
using System.Linq;
using Tests.Characters.Humanoid;
using Tests.Interaction;
using Tests.Interaction.Influence;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Characters.Interaction
{
    public abstract class CharacterBase : MonoBehaviour, ICharacter
    {
        Guid _id;
        InteractableItem _item;
        Blackboard _blackboard;
        InfluenceCore _influenceCore;
        CharacterBehavioursStatemachine _statemachine;
        public CharacterBase()
        {
            _id = Guid.NewGuid();

        }
        protected virtual void Awake()
        {
            _blackboard = CreateBlackboard();
            _influenceCore = CreateInfluenceCore();

        }
        protected virtual void Start()
        {
            var normalState = InitializeController(_blackboard);
            _statemachine = CreateStatemachine(normalState, _influenceCore);
        }

        protected virtual void OnEnable()
        {
            TryRegisterToInteractionManager();
            _statemachine.Enabled = true;
        }
        protected virtual void OnDisable()
        {
            _statemachine.Enabled = false;
            UnregisterFromInteractionManager();
        }
        protected virtual void Update()
        {
            _statemachine.OnUpdate();
        }
        void TryRegisterToInteractionManager()
        {
            var attrs = this.GetType().GetCustomAttributes(true);
            var a = attrs.FirstOrDefault(a => a is InteractableAttribute);
            if (a != null)
            {
                _item = new InteractableItem(this.ID, this.gameObject);
                InteractionManager.AddItem(_item);
            }
        }
        void UnregisterFromInteractionManager()
        {
            InteractionManager.RemoveItem(_item);
        }
        public Guid ID => _id;

        public string Name => this.name;
        internal virtual Blackboard CreateBlackboard()
        {
            var blackboard = new Blackboard();
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Influence_Core, _influenceCore);
            return blackboard;
        }
        internal abstract InfluenceCore CreateInfluenceCore();
        internal abstract NormalState InitializeController(Blackboard blackboard);
        internal abstract CharacterBehavioursStatemachine CreateStatemachine(NormalState normalState, InfluenceCore influenceCore);
    }
}
