using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Tests.Scripts.Locomotion
{

    internal class GetClosePoint : MonoBehaviour
    {
        [SerializeField]
        GameObject _obj_0;
        //[SerializeField]
        //GameObject _obj_1;
        Vector3 _p0;
        //Vector3 _p1;
        private void FixedUpdate()
        {
            _p0 = _obj_0.GetComponent<Collider>().ClosestPoint(this.transform.position);
            //_p1 = _obj_1.GetComponent<Collider>().ClosestPoint(_obj_0.transform.position);
        }
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_p0, this.transform.position);
        }
    }
}
