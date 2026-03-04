using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MNAC.Interaction
{
    [Serializable]
    public class GameObjsInScreenCatcher : CatcherBase<GameObject>
    {
        [SerializeField]
        ushort _processingAmountOfFrames;
        [SerializeField]
        Camera _camera;
        Action<List<GameObject>> _catchCompletedAction;
        public GameObjsInScreenCatcher(Camera camera, ushort processingAmountInCoroutine = 30)
        {
            _processingAmountOfFrames = processingAmountInCoroutine;
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            InteractionManager.itemRemovedAction += item => RemoveItemImpl(item.Obj); //TODO: 啥意思???
        }
        public Camera Camera { get => _camera; set => _camera = value ?? throw new NullReferenceException(nameof(_camera)); }
        public ushort ProcessingAmountOfFrames { get => _processingAmountOfFrames; set => _processingAmountOfFrames = value; }
        public Action<List<GameObject>> CatchCompletedAction { get => _catchCompletedAction; set => _catchCompletedAction = value; }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (value)
                {
                    InteractionManager.handlerList.Add(Handler);
                    InteractionManager.CoroutineEndAction += WhenCoroutineEnd;
                }
                else
                {
                    InteractionManager.handlerList.Remove(Handler);
                    InteractionManager.CoroutineEndAction -= WhenCoroutineEnd;
                }
            }
        }
        void Handler(IInteractable item)
        {
            if (item is not IVolumetricInteractable nitem)
                return;
            var obj = item.Obj;
            var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
            if (obj == null)
                return;
            if (caughtItems.Contains(obj))
            {
                if (!GeometryUtility.TestPlanesAABB(planes, nitem.Bounds))
                {
                    RemoveItemImpl(obj);
                }
            }
            else
            {
                if (GeometryUtility.TestPlanesAABB(planes, nitem.Bounds))
                {

                    AddItemImpl(obj);
                }
            }
        }
        void WhenCoroutineEnd()
        {
            _catchCompletedAction?.Invoke(caughtItems);
        }
    }
}
