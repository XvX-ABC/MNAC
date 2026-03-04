using MNAC.Utilities.MTrees;

namespace MNAC.Utilities.Composable
{
    public interface IComponentNode<T> : IMTContainerNode<IComponent<T>>
    {

    }
    public interface IComponentNode : IMTContainerNode<IComponent>
    {
    }
}
