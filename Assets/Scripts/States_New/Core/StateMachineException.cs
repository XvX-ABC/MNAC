using System;

namespace MNAC.StatesNew
{
    /// <summary>状态机异常基类。</summary>
    public class StateMachineException : Exception
    {
        public StateMachineException(string message, Exception inner = null) : base(message, inner)
        {
        }
    }

    /// <summary>状态未注册到当前状态机（添加转移/切换时校验）。</summary>
    public class StateNotRegisteredException : StateMachineException
    {
        public StateNotRegisteredException(string stateName)
            : base($"State '{stateName}' is not registered in the state machine.")
        {
        }
    }

    /// <summary>状态 OnExit 抛出异常时包装，携带状态名。</summary>
    public class StateExitException : StateMachineException
    {
        public StateExitException(string stateName, Exception inner)
            : base($"Exception thrown when exiting state '{stateName}': {inner?.Message}", inner)
        {
        }
    }

    /// <summary>状态 OnEnter 抛出异常时包装，携带状态名。</summary>
    public class StateEnterException : StateMachineException
    {
        public StateEnterException(string stateName, Exception inner)
            : base($"Exception thrown when entering state '{stateName}': {inner?.Message}", inner)
        {
        }
    }

    /// <summary>状态 OnUpdate 抛出异常时包装，携带状态名。</summary>
    public class StateUpdateException : StateMachineException
    {
        public StateUpdateException(string stateName, Exception inner)
            : base($"Exception thrown when updating state '{stateName}': {inner?.Message}", inner)
        {
        }
    }
}
