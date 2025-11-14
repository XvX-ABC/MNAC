using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Interaction;
using Tests.UI;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    internal class TargetLocker_Test : MonoBehaviour
    {
        [SerializeField]
        Camera _camera;
        [SerializeField]
        IndicatorsManager _manager;
        [SerializeField]
        RingCatcher _ringCatcher;
        [SerializeField]
        ushort _handleAmountInCoroutine = 30;
        [SerializeField]
        GameObject _actor;

        GameObjsInScreenCatcher_New _screenObjsCatcher;
        TargetLocker _locker;


        Vector3 _lastPos;
        private void Awake()
        {
            _screenObjsCatcher = new(_camera, _handleAmountInCoroutine);
            _ringCatcher.Camera = _camera;
            _locker = new(_screenObjsCatcher, _camera, _manager, _ringCatcher, _handleAmountInCoroutine);
        }
        private void Start()
        {
            StartCoroutine(_screenObjsCatcher.UpdateWithCoroutine());
        }
        private void OnEnable()
        {
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
            _locker.OriginPosition = _camera.WorldToScreenPoint(_actor.transform.position);
            //_locker.CursorPositionDelta = UnityEngine.Input.mousePositionDelta;
            _locker.Update();


            //_locker.CursorPosition = UnityEngine.Input.mousePosition;
            //_locker.OriginPosition = _camera.WorldToScreenPoint(_actor.transform.position);
            //_locker.CursorPositionDelta = UnityEngine.Input.mousePositionDelta;
            //_screenObjsCatcher.Update();
            //_locker.FixedUpdate();
        }
        private void LateUpdate()
        {
            _locker.LateUpdate();
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
                GUILayout.Label("Cursor Position: " + _locker.CursorPosition);
                GUILayout.Label("Origin Position: " + _locker.OriginPosition);
                GUILayout.Label("Catch Angle: " + _locker.CatchAngle);
                GUILayout.Label("Catch Direction: " + _locker.cursorPositionDeltaCache.normalized);
                GUILayout.Label("Current Num: " + _locker._num);
                GUILayout.Label("Enabled: " + _locker.Enabled);
                GUILayout.Label("Main Target Name: : " + _locker.MainTargetObj);
                GUILayout.Label(_locker.statemachine.ToString());
                GUILayout.EndVertical();

            }

            _ringCatcher.OnGUIImpl();
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
            _locker?.OnDrawGizmos();
        }
    }
}
