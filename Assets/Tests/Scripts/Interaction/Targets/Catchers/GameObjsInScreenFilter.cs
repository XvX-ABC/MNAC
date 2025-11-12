using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    [Serializable]
    public class GameObjsInScreenFilter
    {
        [SerializeField]
        ushort _filterAmountInCoroutine;
        [SerializeField]
        Camera _camera;
        List<GameObject> _objsInScreen;
        bool _enabled;
        private GameObjsInScreenFilter()
        {

        }
        public GameObjsInScreenFilter(Camera camera, ushort filterCountOneFrame)
        {
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            FilterAmountInCoroutine = filterCountOneFrame;
            _objsInScreen = new List<GameObject>();
        }

        public bool Enabled { get => _enabled; set => _enabled = value; }
        public IReadOnlyList<GameObject> ObjsInScreen { get => _objsInScreen; }
        internal ushort FilterAmountInCoroutine { get => _filterAmountInCoroutine; set => _filterAmountInCoroutine = (ushort)Mathf.Max(0, value); }
        protected int GetAllObjsAmount()
        {
            return InteractionManager.Count;
        }
        public IEnumerator Update()
        {
            while (true)
            {
                if (!Enabled)
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
}
