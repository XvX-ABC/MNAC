using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Characters.Locomotion
{
    public interface IMovementDefinitions
    {
        public float MaxSpeed { get; }
        public float AcceleratedSpeed { get; }
    }
    public interface IWalkingDefinitions : IMovementDefinitions
    {
    }
    public interface IBoostingDefinitions
    {
        public float AcceleratedSpeedPower { get; }
        public float MaxSpeedPower { get; }
    }
}
