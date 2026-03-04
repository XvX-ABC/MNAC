using System.Collections.Generic;
using UnityEngine;

namespace MNAC.TPhysics.Environment
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GroundDetectionDefinitions_MonoComponent))]
    public class GroundDetector_MonoComponent : MonoBehaviour, IGroundDetector
    {
        GroundDetector _detector;
        Rigidbody _rbody;
        public bool Enabled { get => _detector.Enabled; set => _detector.Enabled = value; }

        public IReadOnlyList<Ground> Grounds => _detector.Grounds;

        public GroundVerticalProbe Probe => _detector.Probe;

        public Vector3 GroundsNormal => _detector.GroundsNormal;

        void Awake()
        {
            var definitions = GetComponent<IGroundDetectionDefinitions>();
            _rbody = GetComponent<Rigidbody>();
            _detector = new(definitions.MaxSlope, definitions.GroundMask);
        }
        void OnEnable()
        {
            _detector.Enabled = true;
        }
        void OnDisable()
        {
            _detector.Enabled = false;
        }
        private void OnCollisionEnter(Collision collision)
        {
            _detector.OnCollisionEnter(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            _detector.OnCollisionExit(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            _detector.OnCollisionStay(collision);
        }
        void FixedUpdate()
        {
            _detector.Position = _rbody.position;
            _detector.OnFixedUpdate();
        }

        public void OnLateUpdate()
        {
            _detector.OnLateUpdate();
        }
    }
}
