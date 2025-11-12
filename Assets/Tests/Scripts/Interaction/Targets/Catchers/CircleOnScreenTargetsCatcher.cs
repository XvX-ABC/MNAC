using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    [Serializable]
    public class CircleOnScreenTargetsCatcher : TargetsCatcherBase_New<GameObjTarget>
    {
        [SerializeField]
        protected Camera camera;
        protected Vector3 mousePosition;
        protected float viewPortRadius;
        protected float viewPortRadiusSqr;
        protected float screenPixelRatio;

        [SerializeField]
        protected ushort handleAmountInCoroutine;
        [SerializeField]
        protected GameObject ownerObj;
        [SerializeField]
        protected LayerMask targetsMask;


        protected IReadOnlyList<GameObject> objsInScreen;
        private CircleOnScreenTargetsCatcher()
        {

        }
        public CircleOnScreenTargetsCatcher(IReadOnlyList<GameObject> objsInScreen, GameObject ownerObj, Camera camera, LayerMask targetsMask, ushort handleAmountInCoroutine)
        {
            this.objsInScreen = objsInScreen ?? throw new ArgumentNullException(nameof(objsInScreen));
            this.ownerObj = ownerObj ?? throw new ArgumentNullException(nameof(ownerObj));
            this.targetsMask = targetsMask;
            HandleAmountInCoroutine = handleAmountInCoroutine;
            this.camera = camera;
            screenPixelRatio = this.camera.pixelRect.width / this.camera.pixelRect.height;
        }
        internal ushort HandleAmountInCoroutine { get => handleAmountInCoroutine; set => handleAmountInCoroutine = (ushort)Mathf.Max(0, value); }
        public Vector3 MousePosition { get => mousePosition; set => mousePosition = value; }
        public float Radius
        {
            get => viewPortRadius;
            set
            {
                viewPortRadius = Mathf.Max(0, value);
                viewPortRadiusSqr = viewPortRadius * viewPortRadius;
            }
        }

        public IReadOnlyList<GameObject> ObjsInScreen { get => objsInScreen; set => objsInScreen = value; }

        protected bool MaskCheck(GameObject obj)
        {
            return ((1 << obj.layer) & targetsMask) != 0;
        }
        protected bool CheckObjValidity(GameObject obj)
        {
            return obj != null && obj != ownerObj;
        }
        protected bool CheckObjInCatchRadius(GameObject obj)
        {
            var mpos = (Vector2)camera.ScreenToViewportPoint(mousePosition);
            var spos = (Vector2)camera.WorldToViewportPoint(obj.transform.position);
            var dv = mpos - spos;
            dv.x *= screenPixelRatio;
            var length = dv.sqrMagnitude;
            return length <= viewPortRadius;
        }
        //TODO: 工作逻辑需要优化
        //UNDONE: 等待测试
        public override IEnumerator UpdateWithCoroutine()
        {
            while (true)
            {
                if (!enabled)
                {
                    CleanAllTargets();
                    yield return null;
                }
                for (int i = 0; i < objsInScreen.Count; i++)
                {
                    var obj = objsInScreen[i];
                    if (CheckObjValidity(obj))
                    {
                        var idx = targets.FindIndex(t => t.obj == obj);
                        var target = idx > -1 ? targets[idx] : null;
                        if (idx > -1)
                        {
                            if (!CheckObjInCatchRadius(obj) || !MaskCheck(obj))
                            {
                                RemoveTarget(target);
                                GameObjTarget.ReleaseInstance(target);
                            }
                        }
                        else
                        {
                            if (CheckObjInCatchRadius(obj) && MaskCheck(obj))
                                AddTarget(GameObjTarget.GetInstance(obj));
                        }
                    }
                    if (i > 0 && i % handleAmountInCoroutine == 0)
                        yield return null;
                }
                yield return null;
            }
        }
    }
}
