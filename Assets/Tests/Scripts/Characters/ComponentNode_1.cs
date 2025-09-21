namespace Tests.Characters.Interaction
{
    internal abstract class ComponentNode<T> : ComponentBase
    {
        internal abstract T component { get; }
    }

}
