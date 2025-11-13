using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using Tests.Interaction;
using Tests.UI;
using Tests.Utilities.Timeline;
using UnityEditor;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms.Weapons.Launchers
{
    internal class TargetLocker
    {
        GameObjsInScreenCatcher_New _screenObjsCatcher;
        Camera _camera;

        IndicatorsManager _indicatorManager;
        RingCatcher _ringCatcher;

        GameObject _mainTargetObj;
        Vector3 _originPosition;
        Vector3 _cursorPosition;
        Vector3 _cursorPositionDelta;
        Timeline_V1 _cdTimeline;
        internal Vector3 _cursorPositionDeltaCache;
        float _catchAngle;
        internal byte _num;
        public bool Enabled
        {
            get => _screenObjsCatcher.Enabled;
            set
            {
                _screenObjsCatcher.Enabled = value;
                _ringCatcher.HIde = !value;
            }
        }

        public Vector3 OriginPosition { get => _originPosition; set => _originPosition = value; }
        public Vector3 CursorPosition { get => _cursorPosition; set => _cursorPosition = value; }
        public Vector3 CursorPositionDelta { get => _cursorPositionDelta; set => _cursorPositionDelta = value; }
        public GameObject MainTargetObj { get => _mainTargetObj; set => _mainTargetObj = value; }
        public float CatchAngle { get => _catchAngle * 2; set => _catchAngle = value / 2; }

        public TargetLocker(GameObjsInScreenCatcher_New screenObjsCatcher, Camera camera, IndicatorsManager indicatorsManager, RingCatcher ringCatcher, ushort handleAmountInCoroutine = 30, float catchAngle = 60, bool enabled = true)
        {
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _indicatorManager = indicatorsManager;
            _screenObjsCatcher = screenObjsCatcher ?? throw new ArgumentNullException(nameof(screenObjsCatcher));
            _ringCatcher = ringCatcher;

            CatchAngle = catchAngle;

            _screenObjsCatcher.ItemCaughtAction += WhenCaughtItem;
            _screenObjsCatcher.ItemReleaseAction += WhenReleaseItem;
            _screenObjsCatcher.CatchCompletedAction += WhenCatchCompleted;

            Enabled = enabled;

            _cdTimeline = new Timeline_V1(0.3f);
        }
        void WhenCaughtItem(GameObject obj)
        {
            _indicatorManager?.AddTargetFor<IndicatedTarget>(obj);
        }
        void WhenReleaseItem(GameObject obj)
        {
            _indicatorManager?.RemoveTargetFor<IndicatedTarget>(obj);
            if (obj == _mainTargetObj)
            {
                MainTargetObj = null;
                _num = 0;
            }
        }
        void WhenCatchCompleted(List<GameObject> caughtObjs)
        {
            if (_num == 0)
            {
                MainTargetObj = GetClosestObj(caughtObjs);
                _num = 2;
            }
            else if (_num == 1)
            {
                var obj = FindClosestObjByMainObj(caughtObjs, _cursorPositionDeltaCache.normalized);
                if (obj != null)
                    MainTargetObj = obj;
                _num = 2;
            }
        }
        GameObject GetClosestObj(List<GameObject> objs)
        {
            if (objs.Count == 0)
                return null;
            var minDistance = float.MaxValue;
            var closestObj = default(GameObject);
            var screenPos = (Vector2)_originPosition;
            foreach (var obj in objs)
            {
                var pos = (Vector2)_camera.WorldToScreenPoint(obj.transform.position);
                var distance = Vector3.Distance(pos, screenPos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestObj = obj;
                }
            }
            return closestObj;
        }
        void UpdateRingCatcher()
        {
            _ringCatcher.CursorPosition = _mainTargetObj != null ? _camera.WorldToScreenPoint(_mainTargetObj.transform.position) : _cursorPosition;
        }
        GameObject FindClosestObjByMainObj(List<GameObject> objs, Vector2 direction)
        {
            if (direction == Vector2.zero)
                return null;
            var sb = new StringBuilder();
            sb.AppendLine($"main obj: {_mainTargetObj.name}");
            sb.AppendLine($"direction: {direction}");
            var originalPos = (Vector2)_camera.WorldToScreenPoint(_mainTargetObj.transform.position);
            var minDistance = float.MaxValue;
            var closestObj = default(GameObject);
            for (int i = 0; i < objs.Count; i++)
            {
                var obj = objs[i];
                if (obj == _mainTargetObj)
                    continue;
                var pos = (Vector2)_camera.WorldToScreenPoint(obj.transform.position);
                var tv = pos - originalPos;
                //var tv = originalPos - pos;
                var angle = Vector2.Angle(direction, tv);
                sb.AppendLine($"obj: {obj.name} angle: {angle}");
                if (angle > _catchAngle)
                    continue;
                var distance = Vector2.Distance(pos, originalPos);
                sb.AppendLine($"distance: {distance}");
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestObj = obj;
                }
            }
            if (closestObj != null)
                sb.AppendLine($"closest obj: {closestObj.name}");
            Debug.Log(sb.ToString());
            return closestObj;
        }
        GameObject FindClosestObjByMainObj_0(List<GameObject> objs, Vector2 direction)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"main obj: {_mainTargetObj.name}");
            sb.AppendLine($"direction: {direction}");
            var originalPos = (Vector2)_camera.WorldToScreenPoint(_mainTargetObj.transform.position);
            var minDistance = float.MaxValue;
            var closestObj = default(GameObject);
            for (int i = 0; i < objs.Count; i++)
            {
                var obj = objs[i];
                if (obj == _mainTargetObj)
                    continue;
                var pos = (Vector2)_camera.WorldToScreenPoint(obj.transform.position);
                var tv = pos - originalPos;

                //var v = Vector3.Project(tv, direction);
                //var v0 = ((Vector2)v) - tv;
                var v0 = Vector3.Dot(tv, direction);
                var distance = v0 * 0.7f + tv.magnitude * 0.3f;
                sb.AppendLine($"obj: {obj.name} tv: {tv.magnitude} v: {v0}  v0: {v0 * 0.7f}  tv0: {tv.magnitude * 0.3f} distance: {distance}");
                if (-distance < minDistance)
                {
                    minDistance = distance;
                    closestObj = obj;

                }
            }
            if (closestObj != null)
                sb.AppendLine($"closest obj: {closestObj.name}");
            Debug.Log(sb.ToString());
            return closestObj;
        }
        public void Update()
        {
            if (_num == 2 && _cursorPositionDelta != Vector3.zero)
            {
                //_num = 1;
                //_cursorPositionDeltaCache = _cursorPositionDelta;
                if (count == 0)
                    pos.Clear();
                pos.Add(_cursorPositionDelta);
                count++;
                Debug.Log("count: " + count);
            }
            else
            {
                count = 0;
            }

            if (count > 5)
            {
                Debug.Log("start find");
                for (int i = 1; i < pos.Count; i++)
                {
                    _cursorPositionDeltaCache += pos[i];
                }
                _cursorPositionDeltaCache /= pos.Count;
                _num = 1;
                count = 0;
            }
            //if (_num == 3 && _cursorPositionDelta == Vector3.zero)
            //{
            //    _num = 2;
            //    _cdTimeline.Restart();
            //}
            //_cdTimeline.OnUpdate(Time.deltaTime);
        }
        int count = 0;
        List<Vector3> pos = new();
        public void FixedUpdate()
        {
            if (_num == 2 && _cursorPositionDelta != Vector3.zero)
            {
                if (count == 0)
                    pos.Clear();
                pos.Add(_cursorPositionDelta);
                count++;
                Debug.Log("count: " + count);
            }
            else
            {
                count = 0;
            }
            if (count > 5)
            {
                Debug.Log("start find");
                var sum = Vector3.zero;
                for (int i = 1; i < pos.Count; i++)
                {
                    sum += pos[i];
                }
                sum /= pos.Count;

                var obj = FindClosestObjByMainObj(_screenObjsCatcher.CaughtItems.ToList(), sum.normalized);
                if (obj != null)
                    _mainTargetObj = obj;
                //_cdTimeline.Restart();
                count = 0;
                _num = 2;
            }
            _cdTimeline.OnUpdate(Time.deltaTime);
        }
        public void LateUpdate()
        {
            UpdateRingCatcher();
        }
        public void OnDrawGizmos()
        {
            if (!Application.isPlaying || _mainTargetObj == null)
                return;
            var originalPos = (Vector2)_camera.WorldToScreenPoint(_mainTargetObj.transform.position);
            var minDistance = float.MaxValue;
            var closestObj = default(GameObject);
            var direction = _cursorPositionDeltaCache.normalized;
            foreach (var obj in _screenObjsCatcher.CaughtItems)
            {
                if (obj == _mainTargetObj)
                    continue;
                var pos = (Vector2)_camera.WorldToScreenPoint(obj.transform.position);
                var tv = pos - originalPos;
                var angle = Vector2.Angle(direction, tv);
                var distance = Vector2.Distance(pos, originalPos);
                Handles.Label(obj.transform.position, $"a: {angle}, d: {distance}");
            }
        }
    }
}
