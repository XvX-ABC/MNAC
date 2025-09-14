using System.Collections.Generic;
using Tests.Input;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using Context = Tests.TPhysics.Locomotion.Context;

namespace Tests.Interaction
{
    internal class LeadingActorTargetsCather : TargetsCatcherBase
    {
        ILeadingActorTargetsCatcherDefinitions _definitions;
        Camera _camera;
        IInput _input;
        LocomotionCore _locomotion;
        GameObject _actorObj;

        TerrainTarget _terrainTarget;
        internal class TerrainTarget : IGameObjTarget
        {
            internal GameObject obj;
            internal Vector3 pos;
            public GameObject Obj => obj;

            public Vector3 Position => pos;
        }
        protected Context context => _locomotion.Context;
        public LeadingActorTargetsCather(ILeadingActorTargetsCatcherDefinitions definitions, GameObject actorObj, Camera camera, IInput input, LocomotionCore locomotion)
        {
            this._actorObj = actorObj;
            _definitions = definitions;
            this._camera = camera;
            this._input = input;
            this._locomotion = locomotion;
            if (_definitions.AllowCatchTerrain)
                _terrainTarget = new();
        }
        protected override List<ITarget> NewTargetsContainer()
        {
            return new List<ITarget>(2);
        }
        bool CatchRangeCheck(Vector3 pos)
        {
            var cpos = context.CurrentPosition;
            var crotation = context.CurrentRotation;
            var tpos = Quaternion.Inverse(crotation) * (pos - cpos);
            var angles = Quaternion.FromToRotation(Vector3.forward, tpos).eulerAngles;
            // top bottom
            var verticalLimit = new Vector2(_definitions.CatchingRange.x, _definitions.CatchingRange.z);
            // right left
            var horizontalLimit = new Vector2(_definitions.CatchingRange.y, _definitions.CatchingRange.w);
            return (angles.x >= 360 - Mathf.Abs(verticalLimit.x) || angles.x <= Mathf.Abs(verticalLimit.y)) && (angles.y <= Mathf.Abs(horizontalLimit.y) || angles.y >= 360 - Mathf.Abs(horizontalLimit.x));
        }
        public override void Update()
        {
            var pos = context.CurrentPosition;
            var ray = _camera.ScreenPointToRay(_input.MousePosition);
            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _definitions.TerrainMask | _definitions.TargetsMask))
            {
                var point = hitInfo.point;
                var obj = hitInfo.collider.gameObject;
                if (obj == _actorObj)
                {
                    ClearAllTargets();
                    return;
                }
                var mask = 1 << obj.layer;
                if (!CatchRangeCheck(point))
                {
                    ClearAllTargets();
                    return;
                }
                if (_definitions.AllowCatchTerrain && (mask & _definitions.TerrainMask) > 0)
                {
                    _terrainTarget.obj = obj;
                    _terrainTarget.pos = point;
                    if (targets.Count == 0)
                        AddTarget(_terrainTarget);
                    else
                    {
                        ClearAllTargets();
                        AddTarget(_terrainTarget);
                    }
                }
                else if ((mask & _definitions.TargetsMask) > 0)
                {
                    var index = targets.FindIndex(t => t is GameObjTarget gt && gt.obj == obj);
                    if (index == -1)
                    {
                        var target = new GameObjTarget() { obj = obj };
                        AddTarget(target);
                    }
                }
                else
                    ClearAllTargets();
            }
            else
                ClearAllTargets();
        }
    }
}
