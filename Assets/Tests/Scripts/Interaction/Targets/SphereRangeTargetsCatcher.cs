using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    internal class GameObjsInScreenFilter
    {
        string _tag;
        ushort _filterCountInCoroutine;
        Camera _camera;
        List<GameObject> _objsInScreen;
        public GameObjsInScreenFilter(string tag, Camera camera, ushort filterCountOneFrame)
        {
            _tag = tag ?? throw new ArgumentNullException(nameof(tag));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            FilterCountInCoroutine = filterCountOneFrame;
            _objsInScreen = new List<GameObject>();
        }

        public IReadOnlyList<GameObject> ObjsInScreen { get => _objsInScreen; }
        internal ushort FilterCountInCoroutine { get => _filterCountInCoroutine; set => _filterCountInCoroutine = (ushort)Mathf.Max(0, value); }

        public IEnumerator Update()
        {
            _objsInScreen.Clear();
            if (_tag == null || _tag.Length == 0)
                throw new NullReferenceException(nameof(_tag));
            var objs = GameObject.FindGameObjectsWithTag(_tag);
            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                if (obj.TryGetComponent<Renderer>(out var renderer))
                {
                    var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                    if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
                        _objsInScreen.Add(obj);
                }
                if (i % _filterCountInCoroutine == 0)
                    yield return null;
            }
        }
    }
    internal class CircleRangeTargetsCatcher : TargetsCatcherBase
    {
        Camera _camera;
        Vector3 _actorPosition;
        Vector3 _mousePosition;
        float _viewPortRadius;
        float _viewPortRadiusSqr;

        ushort _filterCountInCoroutine;
        GameObject _ownerObj;
        LayerMask _targetsMask;

        IReadOnlyList<GameObject> _objsInScreen;
        public CircleRangeTargetsCatcher(IReadOnlyList<GameObject> objsInScreen, GameObject ownerObj, LayerMask targetsMask, ushort filterCountOneFrame)
        {
            _objsInScreen = objsInScreen ?? throw new ArgumentNullException(nameof(objsInScreen));
            _ownerObj = ownerObj ?? throw new ArgumentNullException(nameof(ownerObj));
            _targetsMask = targetsMask;
            FilterCountOneFrame = filterCountOneFrame;
        }
        internal ushort FilterCountOneFrame { get => _filterCountInCoroutine; set => _filterCountInCoroutine = (ushort)Mathf.Max(0, value); }
        public Vector3 ActorPosition { get => _actorPosition; set => _actorPosition = value; }
        public Vector3 MousePosition { get => _mousePosition; set => _mousePosition = value; }
        public float ViewPortRadius
        {
            get => _viewPortRadius;
            set
            {
                _viewPortRadius = value;
                _viewPortRadiusSqr = value * value;
            }
        }
        bool MaskCheck(GameObject obj)
        {
            return ((1 << obj.layer) & _targetsMask) != 0;
        }
        public override IEnumerator UpdateWithCoroutine()
        {
            CleanAllTargets();
            for (int i = 0; i < _objsInScreen.Count; i++)
            {
                var obj = _objsInScreen[i];
                if (obj != null && obj != _ownerObj && MaskCheck(obj))
                {
                    var mpos = _camera.ScreenToViewportPoint(_mousePosition);
                    var spos = _camera.WorldToViewportPoint(obj.transform.position);
                    if ((mpos - spos).sqrMagnitude <= _viewPortRadiusSqr)
                        AddTarget(new GameObjTarget(obj));
                }
                if (i % _filterCountInCoroutine == 0)
                    yield return null;
            }
        }
    }
}
