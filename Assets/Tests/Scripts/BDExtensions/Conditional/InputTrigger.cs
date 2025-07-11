using Assets.Tests.Scripts.BDExtensions.Variables;
using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Input;
using Unity.VisualScripting;
using UnityEngine;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;


namespace Assets.Tests.Scripts.BTD
{
    public class InputTrigger : Conditional
    {
        [SerializeField]
        SharedHybridInput _input;
        [SerializeField]
        SharedCustomInputTypes _inputType;
        public override TaskStatus OnUpdate()
        {
            var type = _inputType.Value;
            var input = _input.Value;
            switch (type)
            {
                case InputTypes.HorizontalDirection:
                    if (input.HorizontalDirection != Vector3.zero)
                        return TaskStatus.Success;
                    break;
                case InputTypes.IsAscending:
                    if (input.IsAscending)
                        return TaskStatus.Success;
                    break;
                case InputTypes.IsBoosting:
                    if (input.IsBoosting)
                        return TaskStatus.Success;
                    break;
                case InputTypes.Fire:
                    if (input.Fire)
                        return TaskStatus.Success;
                    break;
                case InputTypes.Reload:
                    if (input.Reload)
                    {
                        Debug.Log("Reload Triggered");

                        return TaskStatus.Success;
                    }
                    break;
                case InputTypes.Supply:
                    if (input.Supply)
                        return TaskStatus.Success;
                    break;
            }
            return TaskStatus.Failure;
        }
    }
}
