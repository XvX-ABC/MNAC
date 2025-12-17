using Tests.Utilities.MTrees;

namespace Tests.Utilities.Composable
{
    public interface IComponentNode<T> : IMTContainerNode<IComponent<T>>
    {

    }
    public interface IComponentNode : IMTContainerNode<IComponent>
    {
    }
}
