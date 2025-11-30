//using System;
//using UnityEngine;

//namespace Locomotion
//{
//    public class LocomotionWithPlayerInput : LocomotionControlBase
//    {
//        class FrameContext : IFrameContext
//        {
//            internal Vector3 _targetPos;
//            Vector3 _currentVelocity;
//            Vector3 _rbCurrentPos;
//            Rigidbody _rb;
//            Camera _camera;

//            public FrameContext(Rigidbody rb, Camera camera)
//            {
//                _rb = rb;
//                _camera = camera;
//            }

//            public Vector3 TargetPos
//            {
//                get
//                {
//                    _camera.GetComponent<ThirdPersonCameraController>().UpdatePos(_rb.position);
//                    var ray = _camera.ScreenPointToRay(Input.mousePosition);
//                    if (Physics.Raycast(ray, out var hitInfo))
//                    {
//                        return hitInfo.point;
//                    }
//                    return default;
//                }
//            }

//            public Vector3 CurrentPos
//            {
//                get
//                {
//                    return _rb.position;
//                }
//            }

//            public Vector3 CurrentVelocity => _currentVelocity;

//            public void Update()
//            {
//                _currentVelocity = _rb.velocity;
//            }
//        }
//        FrameContext _currentFrameContext;
//        [SerializeField]
//        Camera _camera;
//        internal override IFrameContext CurrentFrameContext => _currentFrameContext;

//        internal override bool IsForward => Input.GetKey(KeyCode.W);

//        internal override bool IsBack => Input.GetKey(KeyCode.S);

//        internal override bool IsRight => Input.GetKey(KeyCode.D);

//        internal override bool IsLeft => Input.GetKey(KeyCode.A);

//        internal override bool IsAscending => Input.GetKey(KeyCode.Space);

//        internal override bool TriggeredQuickBoost => Input.GetKey(KeyCode.Mouse1);

//        protected new void Awake()
//        {
//            LoadRequirementComponents();
//            _currentFrameContext = new(rb, _camera);
//            InitializeLocomotionObjs();
//        }
//        new void Update()
//        {
//            base.Update();
//        }
//        protected override void OnDrawGizmosSelected()
//        {
//            base.OnDrawGizmosSelected();
//            //if (!Application.isPlaying)
//            //    return;
//            //var ray = _camera.ScreenPointToRay(Input.mousePosition);
//            //if (Physics.Raycast(ray, out var hitInfo))
//            //{
//            //    Gizmos.DrawCube(hitInfo.point, Vector3.one * 0.6f);
//            //}
//        }
//    }
//}
