using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    [Serializable]
    public class GameObjsInScreenCatcher
    {
        [SerializeField]
        ushort _handleAmountInCoroutine;
        [SerializeField]
        Camera _camera;
        List<GameObject> _objsInScreen;
        bool _enabled;
        private GameObjsInScreenCatcher()
        {

        }
        public GameObjsInScreenCatcher(Camera camera, ushort filterCountOneFrame)
        {
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            HandleAmountInCoroutine = filterCountOneFrame;
            _objsInScreen = new List<GameObject>();
        }

        public bool Enabled { get => _enabled; set => _enabled = value; }
        public IReadOnlyList<GameObject> ObjsInScreen { get => _objsInScreen; }
        public ushort HandleAmountInCoroutine { get => _handleAmountInCoroutine; set => _handleAmountInCoroutine = (ushort)Mathf.Max(0, value); }
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
                    if (i > 0 && i % _handleAmountInCoroutine == 0)
                        yield return null;
                    i++;
                }
                yield return null;
            }
        }
    }
}
