using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    internal class GameObjsInScreenFilter
    {
        string _tag;
        ushort _filterAmountInCoroutine;
        Camera _camera;
        List<GameObject> _objsInScreen;
        internal bool enabled;
        public GameObjsInScreenFilter(string tag, Camera camera, ushort filterCountOneFrame)
        {
            _tag = tag ?? throw new ArgumentNullException(nameof(tag));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            FilterAmountInCoroutine = filterCountOneFrame;
            _objsInScreen = new List<GameObject>();
        }

        public IReadOnlyList<GameObject> ObjsInScreen { get => _objsInScreen; }
        internal ushort FilterAmountInCoroutine { get => _filterAmountInCoroutine; set => _filterAmountInCoroutine = (ushort)Mathf.Max(0, value); }
        protected GameObject[] GetAllObjs()
        {
            return GameObject.FindGameObjectsWithTag(_tag);
        }
        protected int GetAllObjsAmount()
        {
            return InteractionManager.Count;
        }
        public IEnumerator Update()
        {
            while (true)
            {
                if (!enabled)
                    yield return null;
                _objsInScreen.Clear();
                var length = InteractionManager.Count;
                var i = 0;
                foreach (var item in InteractionManager.items)
                {
                    var obj = item.Obj;
                    if (obj.TryGetComponent<Renderer>(out var renderer))
                    {
                        var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                        if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
                        {
                            _objsInScreen.Add(obj);
                        }
                    }
                    if (i > 0 && i % _filterAmountInCoroutine == 0)
                        yield return null;
                    i++;
                }
                yield return null;
            }
        }
    }
    internal class CircleOnScreenTargetsCatcher : TargetsCatcherBase
    {
        Camera _camera;
        Vector3 _actorPosition;
        Vector3 _mousePosition;
        float _viewPortRadius;
        float _viewPortRadiusSqr;
        float _screenPixelRatio;

        ushort _filterCountInCoroutine;
        GameObject _ownerObj;
        LayerMask _targetsMask;

        IReadOnlyList<GameObject> _objsInScreen;
        public CircleOnScreenTargetsCatcher(IReadOnlyList<GameObject> objsInScreen, GameObject ownerObj, Camera camera, LayerMask targetsMask, ushort filterCountOneFrame)
        {
            _objsInScreen = objsInScreen ?? throw new ArgumentNullException(nameof(objsInScreen));
            _ownerObj = ownerObj ?? throw new ArgumentNullException(nameof(ownerObj));
            _targetsMask = targetsMask;
            FilterCountOneFrame = filterCountOneFrame;
            _camera = camera;
            _screenPixelRatio = _camera.pixelRect.width / _camera.pixelRect.height;
        }
        internal ushort FilterCountOneFrame { get => _filterCountInCoroutine; set => _filterCountInCoroutine = (ushort)Mathf.Max(0, value); }
        public Vector3 ActorPosition { get => _actorPosition; set => _actorPosition = value; }
        public Vector3 MousePosition { get => _mousePosition; set => _mousePosition = value; }
        public float Radius
        {
            get => _viewPortRadius;
            set
            {
                _viewPortRadius = Mathf.Max(0, value);
                _viewPortRadiusSqr = _viewPortRadius * _viewPortRadius;
            }
        }


        bool MaskCheck(GameObject obj)
        {
            return ((1 << obj.layer) & _targetsMask) != 0;
        }
        public override IEnumerator UpdateWithCoroutine()
        {
            while (true)
            {
                if (!enabled)
                    yield return null;
                CleanAllTargets();
                for (int i = 0; i < _objsInScreen.Count; i++)
                {
                    var obj = _objsInScreen[i];
                    if (obj != null && obj != _ownerObj && MaskCheck(obj))
                    {
                        var mpos = (Vector2)_camera.ScreenToViewportPoint(_mousePosition);
                        var spos = (Vector2)_camera.WorldToViewportPoint(obj.transform.position);
                        var dv = mpos - spos;
                        dv.x *= _screenPixelRatio;
                        var length = dv.sqrMagnitude;
                        if (length <= _viewPortRadiusSqr)
                            AddTarget(new GameObjTarget(obj));
                    }
                    if (i > 0 && i % _filterCountInCoroutine == 0)
                        yield return null;
                }
                yield return null;
            }
        }
    }
}
