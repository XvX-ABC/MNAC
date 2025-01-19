using System;

namespace Assets.Scripts.Utilities.Timeline.Event.Range
{
    public class NotSupportedEventTypeException : TimelineException
    {
        public NotSupportedEventTypeException(string message) : base(message)
        {

        }
        public NotSupportedEventTypeException(Type type) : this($"The timeline doesn't supported the event type '{type.Name}'.") { }
    }
}
