using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public class RigidBodyShootingTest : MonoBehaviour
    {
        Rigidbody _rb;
        [SerializeField]
        Vector3 _force;
        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
        void FixedUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
                _rb.AddForce(_force, ForceMode.Impulse);
        }
    }
}
