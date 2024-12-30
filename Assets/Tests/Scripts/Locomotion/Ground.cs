using Locomotion;
using System;
using UnityEngine;
namespace Tests.Locomotion
{
    class Ground : IGround
    {
        IGroundSampler _sampler;

        public Ground(IGroundSampler sampler)
        {
            _sampler = sampler ?? throw new NullReferenceException(nameof(sampler));
        }

        public Vector3 Normal => _sampler.Normal;

        public bool Touched => _sampler.IsOnGround;
        public override string ToString()
        {
            return $"Normal: {_sampler.Normal}, Touched: {_sampler.IsOnGround}";
        }
    }
}