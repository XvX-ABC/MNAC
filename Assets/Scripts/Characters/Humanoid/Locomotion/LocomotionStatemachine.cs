using System;
using System.Diagnostics.CodeAnalysis;
using MNAC.States;
using MNAC.TPhysics;
using MNAC.TPhysics.Locomotion;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Locomotion
{
    internal class LocomotionStatemachine : WithCallbackPlayableStatemachine<object>, IEvaluationModule, IState<object>
    {
        public LocomotionStatemachine(string name, [NotNull] LocomotionStateContext context, bool enabled = true) : base($"{name}_locomotion_statemachine", enabled)
        {
            Context = context;
        }


        public int Priority => IEvaluationModule.DEFAULT_PRIORITY;

        public LocomotionModuleState State => throw new NotImplementedException();

        public TPhysics.Locomotion.Context End(TPhysics.Locomotion.Context context)
        {
            throw new NotImplementedException();
        }

        public TPhysics.Locomotion.Context Start(TPhysics.Locomotion.Context context)
        {
            throw new NotImplementedException();
        }

        public TPhysics.Locomotion.Context Update(TPhysics.Locomotion.Context context)
        {
            throw new NotImplementedException();
        }

        TPhysics.Locomotion.Context ILocomotionModule.Update(TPhysics.Locomotion.Context context)
        {
            OnUpdate();
            return context;
        }
    }
}
