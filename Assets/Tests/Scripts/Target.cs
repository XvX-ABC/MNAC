using UnityEngine;

namespace Tests
{
    class Target : MonoBehaviour, ITarget
    {
        [SerializeField]
        GameObject _obj;
        LocomotionContext _context;

        public GameObject Obj { get => _obj; set => _obj = value; }
        public LocomotionContext Locomotion
        {
            get
            {
                _context.Rotation = _obj.transform.rotation;
                _context.Position = _obj.transform.position;
                return _context;
            }
            set => _context = value;
        }
        void Start()
        {
            _context = new();
        }
    }
}