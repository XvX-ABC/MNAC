using System;
using Tests.Utilities.Composable;

namespace Tests.Utilities.Blackboards
{
    public static class BlackboardExtensionForFieldChangeHandler
    {
        public static void RegisterFieldChangeAction<T>(this Blackboard blackboard, object key, Action<FieldEventType, T, T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            blackboard.TryReadValueOrThrowException<FieldChangeHandler>(MiddlewareFields.FieldChangeHandler, out var handler);
            //handler.RegisterAction(key, action);
            handler.RegisterAction_New(key, action);
        }
        //TODO：没必要使用泛型
        public static void UnregisterFieldChangeAction<T>(this Blackboard blackboard, object key, Action<FieldEventType, T, T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            blackboard.TryReadValueOrThrowException<FieldChangeHandler>(MiddlewareFields.FieldChangeHandler, out var handler);
            //handler.UnregisterAction(key, action);
            handler.UnregisterAction_New(key, action);
        }
        public static void RegisterGlobalFieldChangeAction(this Blackboard blackboard, Action<FieldEventType, object, object> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            blackboard.TryReadValueOrThrowException<FieldChangeHandler>(MiddlewareFields.FieldChangeHandler, out var handler);
            handler.RegisterGlobalAction(action);
        }
        public static void UnregisterGlobalFieldChangeAction(this Blackboard blackboard, Action<FieldEventType, object, object> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            blackboard.TryReadValueOrThrowException<FieldChangeHandler>(MiddlewareFields.FieldChangeHandler, out var handler);
            handler.UnregisterGlobalAction(action);
        }
    }
}
