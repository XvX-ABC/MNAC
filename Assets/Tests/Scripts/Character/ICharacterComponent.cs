using System;

namespace Tests.Characters
{
    public interface ICharacterComponent : ICharacterComponentDescriptions
    {
        public ICharacterContext Context { set; }
    }
    public interface IArmedCharacterComponent : ICharacterComponentDescriptions
    {

        public IArmedCharacterContext Context { set; }
    }
    public abstract class CharacterComponentBase : ICharacterComponent
    {
        protected ICharacterContext context;
        Guid _id;
        public Guid ID { get => _id; }
        public virtual ICharacterContext Context
        {
            set
            {
                if (value != context)
                    context = UpdateWhenContextChanged(value);
            }
        }
        protected virtual ICharacterContext UpdateWhenContextChanged(ICharacterContext newContext)
        {
            if (newContext == null)
                return context;
            newContext.RegisterComponent(this);
            context.UnregisterComponent(this);
            return newContext;
        }
        public abstract string Name { get; }
        protected CharacterComponentBase()
        {
            _id = Guid.NewGuid();
        }
    }
    public abstract class ArmedCharacterComponent : CharacterComponentBase, IArmedCharacterComponent
    {
        protected ArmedCharacterComponent() : base()
        {

        }

        IArmedCharacterContext IArmedCharacterComponent.Context { set => Context = value; }
    }
}
