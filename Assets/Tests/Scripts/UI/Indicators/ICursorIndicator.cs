using Tests.Interaction;

namespace Tests.UI
{
    public interface ICursorIndicator : ICursorReceiver
    {
        float Width { get; set; }
    }
}