using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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
            InteractionManager.itemRemovedAction += item => RemoveItemImpl(item.Obj);
        }
        public Camera Camera { get => _camera; set => _camera = value ?? throw new NullReferenceException(nameof(_camera)); }
        public ushort ProcessingAmountOfFrames { get => _processingAmountOfFrames; set => _processingAmountOfFrames = value; }
        public Action<List<GameObject>> CatchCompletedAction { get => _catchCompletedAction; set => _catchCompletedAction = value; }
        public override void Update()
        {
            if (!this.enabled)
            {
                CleanAll();
                return;
            }

            foreach (var item in InteractionManager.items)
            {
                var obj = item.Obj;
                if (caughtItems.Contains(obj))
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
            }
            _catchCompletedAction?.Invoke(caughtItems);
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
                InteractionManager.SynchronizeChanges();
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
