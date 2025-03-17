using System;
using UnityEngine;
namespace Tests.Locomotion
{
    [Obsolete]
    class Ground : IGround
    {
        IGroundDetector _sampler;

        public Ground(IGroundDetector sampler)
        {
            _sampler = sampler ?? throw new NullReferenceException(nameof(sampler));
        }
        //public Vector3 Normal => _sampler.Normal;

        //public bool Touched => _sampler.TouchedGround;

        public GameObject Obj => throw new NotImplementedException();

        public Vector3 Normal => throw new NotImplementedException();

        public bool Touched => throw new NotImplementedException();

        //public override string ToString()
        //{
        //    return $"Normal: {_sampler.Normal}, Touched: {_sampler.TouchedGround}";
        //}
    }
}