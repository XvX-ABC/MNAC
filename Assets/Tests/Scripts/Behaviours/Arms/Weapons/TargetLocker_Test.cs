using Mono.Cecil.Cil;
using Tests.Characters.Humanoid.Arms.Weapons;
using Tests.Interaction;
using Tests.UI;
using UnityEngine;
namespace Tests.Behaviours.Arms.Weapons
{
    internal class TargetLocker_Test : MonoBehaviour
    {
        [SerializeField]
        Camera _camera;
        [SerializeField]
        IndicatorsManager _manager;
        [SerializeField]
        CursorIndicator cursorIndicator;
        [SerializeField]
        ushort _handleAmountInCoroutine = 30;
        [SerializeField]
        GameObject _actor;
        [SerializeField]
        ObstacleDetector _obstacleDetector;

        GameObjsInScreenCatcher_New _screenObjsCatcher;
        TargetLocker<LockTarget> _locker;


        Vector3 _lastPos;
        private void Awake()
        {
            //LockTarget.indicatorsManager = _manager;
    
        }
        private void Start()
        {
            _screenObjsCatcher = new(_camera, _handleAmountInCoroutine);
            _locker = new(_screenObjsCatcher, LockTarget.GetInstance, LockTarget.ReleaseInstance, _camera, cursorIndicator, null, _handleAmountInCoroutine);
            StartCoroutine(_screenObjsCatcher.UpdateWithCoroutine());

        }
        private void OnEnable()
        {
            if (_locker != null)
                _locker.Enabled = true;
        }
        private void OnDisable()
        {
            _locker.Enabled = false;
        }
        private void Update()
        {


        }
        private void FixedUpdate()
        {

            _locker.CursorPosition = UnityEngine.Input.mousePosition;
            _locker.OriginWorldPosition = _actor.transform.position;
            _locker.CursorPositionDelta = UnityEngine.Input.mousePositionDelta;
            _locker.OnFixedUpdate();


            //_locker.CursorPosition = UnityEngine.Input.mousePosition;
            //_locker.OriginPosition = _camera.WorldToScreenPoint(_actor.transform.position);
            //_locker.CursorPositionDelta = UnityEngine.Input.mousePositionDelta;
            //_screenObjsCatcher.Update();
            //_locker.FixedUpdate();
        }
        private void LateUpdate()
        {
            _locker.OnLateUpdate();
        }
        bool _showTargetLockerOptions;
        private void OnGUI()
        {
            GUI.backgroundColor = Color.green;
            GUILayout.BeginVertical();
            //if (GUILayout.Button("Show Target Locker Options"))
            //    _showTargetLockerOptions = !_showTargetLockerOptions;
            //if (_showTargetLockerOptions)
            {
                GUILayout.BeginVertical("Targets Locker Tests");
                if (GUILayout.Button("Enable Target Locker"))
                    _locker.Enabled = !_locker.Enabled;
                if (GUILayout.Button("Enable Obstacle Detect"))
                {
                    _locker.ObstacleDetector = _locker.ObstacleDetector == null ? _obstacleDetector : null;
                }
                GUILayout.Label("Cursor Position: " + _locker.CursorPosition);
                GUILayout.Label("Origin Position: " + _locker.OriginWorldPosition);
                GUILayout.Label("Catch Angle: " + _locker.CatchAngle);
                GUILayout.Label("Enabled: " + _locker.Enabled);
                GUILayout.Label("Main Target Name: : " + _locker.MainTargetObj);
                GUILayout.Label("Main Lock Target Name" + _locker.MainLockTarget?.Obj?.name ?? "");
                GUILayout.Label("Main Lock Target Type: " + (_locker.MainLockTarget?.LockType ?? LockType.None).ToString());
                GUILayout.Label("Main Lock Target Indicator Type:" + (_locker.MainLockTarget?.indicator?.Type).ToString() ?? "null");
                GUILayout.EndVertical();

            }

            //GUILayout.BeginVertical();
            //GUILayout.Label("Target Locker Test");
            //GUILayout.Label("Cursor Position: " + _locker.CursorPosition);
            //GUILayout.Label("Origin Position: " + _locker.OriginPosition);
            //GUILayout.Label("Enabled: " + _locker.Enabled);
            //GUILayout.Label("Main Target Name: : " + _locker.MainTargetObj);
            GUILayout.EndVertical();
        }
        private void OnDrawGizmos()
        {
        }
    }
}
