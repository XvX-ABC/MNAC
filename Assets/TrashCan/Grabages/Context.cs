using System;
using UnityEngine;

namespace Assets.Scripts
{
    internal partial class Locomotion
    {
        internal class Context
        {
            internal static bool CheckNormalIsValid(Vector3 normal)
            {
                return Vector3.Distance(normal, Vector3.positiveInfinity) > 10;
            }
            public GameObject Obj;
            public Rigidbody RigidBody;
            public Vector3 NormalOnGround;
            public Vector3 Direction;
            public Vector3 Velocity;
            public Context(GameObject obj)
            {
                Obj = obj;
                this.RigidBody = obj.GetComponent<Rigidbody>() ?? throw new Exception($"Can't find a component by type 'RigidBody' from game object '{obj.name}'");
                Velocity = this.RigidBody.velocity;
            }
            void UpdateNormalOnGround()
            {
                var trans = Obj.transform;
                if (Physics.Raycast(new Ray() { origin = trans.position, direction = -trans.up }, out var hitInfo, 1))
                    NormalOnGround = hitInfo.normal;
                else
                    NormalOnGround = Vector3.positiveInfinity;
            }
            internal void OnFixedUpdateStart()
            {
                Velocity = RigidBody.velocity;
                Direction = Vector3.zero;
                UpdateNormalOnGround();
            }
            internal void OnFixedUpdateEnd()
            {
                RigidBody.velocity = Velocity;
            }
        }

    }
}
