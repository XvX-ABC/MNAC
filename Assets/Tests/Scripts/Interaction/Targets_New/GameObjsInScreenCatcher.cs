using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Tests.Interaction
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
        //public override void Update()
        //{
        //    if (!this.enabled)
        //    {
        //        CleanAll();
        //        return;
        //    }

        //    foreach (var item in InteractionManager.items)
        //    {
        //        var obj = item.Obj;
        //        if (caughtItems.Contains(obj))
        //        {
        //            if (obj.TryGetComponent<Renderer>(out var renderer))
        //            {
        //                var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
        //                if (!GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
        //                {
        //                    RemoveItemImpl(obj);
        //                }
        //            }
        //            else
        //                RemoveItemImpl(obj);
        //        }
        //        else if (obj.TryGetComponent<Renderer>(out var renderer))
        //        {
        //            var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
        //            if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
        //            {
        //                AddItemImpl(obj);
        //            }
        //        }
        //    }
        //    _catchCompletedAction?.Invoke(caughtItems);
        //}
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
        void Handler(GameObject obj)
        {
            if (obj != null && caughtItems.Contains(obj))
            {
                //if (obj.TryGetComponent<Renderer>(out var renderer))
                //{
                //    var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                //    if (!GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
                //    {
                //        RemoveItemImpl(obj);
                //    }
                //}
                //else
                //    RemoveItemImpl(obj);

                if (obj.TryGetComponent<IVolumetricInteractable>(out var item))
                {
                    var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                    if (GeometryUtility.TestPlanesAABB(planes, item.Bounds))
                        RemoveItemImpl(obj);
                }
                else
                    RemoveItemImpl(obj);
            }
            else if (obj.TryGetComponent<IVolumetricInteractable>(out var item))
            {
                var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                if (GeometryUtility.TestPlanesAABB(planes, item.Bounds))
                {
                    AddItemImpl(obj);
                }
                //var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                //if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
                //{
                //    AddItemImpl(obj);
                //}
            }
        }
        public override IEnumerator UpdateWithCoroutine()
        {
            while (true)
            {
                var i = 0;
                //foreach (var ai in InteractionManager.waitingAddition)
                //{
                //    InteractionManager.items.Add(ai);
                //}
                //foreach (var ri in InteractionManager.waitingRemoval)
                //{
                //    RemoveItemImpl(ri.Obj);
                //    InteractionManager.items.Remove(ri);

                //}
                //InteractionManager.waitingAddition.Clear();
                //InteractionManager.waitingRemoval.Clear();
                //InteractionManager.SynchronizeChanges();
                foreach (var item in InteractionManager.items)
                {
                    if (!this.enabled)
                    {
                        CleanAll();
                        break;
                    }
                    var obj = item.Obj;
                    if (obj != null && caughtItems.Contains(obj))
                    {
                        if (obj.TryGetComponent<Renderer>(out var renderer))
                        {
                            var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                            if (!GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
                            {
                                RemoveItemImpl(obj);
                            }
                        }
                        else
                            RemoveItemImpl(obj);
                    }
                    else if (obj.TryGetComponent<Renderer>(out var renderer))
                    {
                        var planes = GeometryUtility.CalculateFrustumPlanes(_camera);
                        if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
                        {
                            AddItemImpl(obj);
                        }
                    }
                    if (i <= 0 || i % _processingAmountOfFrames == 0)
                        yield return null;
                    i++;
                }
                _catchCompletedAction?.Invoke(caughtItems);
                yield return null;
            }
        }
    }
}
