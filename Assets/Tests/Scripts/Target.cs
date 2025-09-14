using System;
using UnityEngine;

namespace Tests
{
    public class Target : MonoBehaviour, ITarget_Obsolete
    {
        [SerializeField]
        GameObject _obj;
        LocomotionContext _context;
        Vector3 _position;
        Quaternion _rotation;
        public GameObject Obj { get => _obj; set => _obj = value; }
        [Obsolete]
        public LocomotionContext Locomotion
        {
            get
            {
                if (_obj != null)
                {
                    _context.Rotation = _obj.transform.rotation;
                    _context.Position = _obj.transform.position;
                }
                return _context;
            }
            set => _context = value;
        }
        //public Vector3 Position { get => _obj.transform.position; set => _obj.transform.position = value; }
        public Vector3 Position
        {
            get
            {
                if (_obj != null)
                {
                    var pos = _obj.transform.position;
                    _position = pos;
                }
                return _position;
            }
            set
            {
                if (_obj != null)
                {
                    _obj.transform.position = value;
                }
                    _position = value;
            }
        }
    }
}