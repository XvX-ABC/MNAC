using System;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Composable;
namespace MNAC.Characters.UI
{
    public static class BlackboardExtension
    {
        public static bool TryRegisterUIBlackboard(this Blackboard blackboard, Blackboard uiBlackboard)
        {
            if (blackboard == null)
                return false;
            return blackboard.TryRegisterField(CharacterUIBlackboardFields.Blackboard_Main, uiBlackboard);
        }
        public static bool TryReadUIValue<T>(this Blackboard blackboard, object key, out T value)
        {
            value = default;
            if (blackboard == null)
                return false;
            if (blackboard.TryReadValue<Blackboard>(CharacterUIBlackboardFields.Blackboard_Main, out var subBlackboard))
                return subBlackboard.TryReadValue<T>(key, out value);
            return false;
        }
        public static void TryReadUIValueOrThrowException<T>(this Blackboard blackboard, object key, out T value)
        {
            if (!blackboard.TryReadUIValue(key, out value))
                throw new Exception($"Key '{key}' not found in blackboard.");
        }
        public static void TryRegisterUIField<T>(this Blackboard blackboard, object key, T value = default)
        {
            if (blackboard == null)
                throw new NullReferenceException(nameof(blackboard));
            if (blackboard.TryReadValue<Blackboard>(CharacterUIBlackboardFields.Blackboard_Main, out var subBlackboard))
            {
                subBlackboard.TryRegisterField(key, value);
            }
        }
        public static void TryUnregisterUIField<T>(this Blackboard blackboard, object key, T value = default)
        {
            if (blackboard == null)
                throw new NullReferenceException(nameof(blackboard));
            if (blackboard.TryReadValue<Blackboard>(CharacterUIBlackboardFields.Blackboard_Main, out var subBlackboard))
            {
                subBlackboard.TryUnregisterField(key, value);
            }
        }
    }
}
