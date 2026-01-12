using DG.Tweening;
using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    [RequireComponent(typeof(Sword))]
    internal class Sword_Test_Obsolete : MonoBehaviour
    {
        [SerializeField]
        float _duration;
        [SerializeField]
        KeyCode _key;
        Quaternion _defaultRotation;
        bool _rotated;

        Sword _sword;
        private void Awake()
        {
            _sword = GetComponent<Sword>();
        }
        private void Start()
        {
            _defaultRotation = transform.rotation;
            _sword.HitAction += HitAction;
        }
        void HitAction(GameObject obj)
        {
            Debug.Log("hit obj: " + obj.name);
        }
        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(_key))
            {
                if (!_rotated)
                {
                    var a = Quaternion.Euler(0, -90, 90);
                    transform.DORotateQuaternion(a, _duration);
                    _rotated = true;
                }
                else
                {
                    transform.DORotateQuaternion(_defaultRotation, _duration);
                    _rotated = false;
                }
            }
        }
    }
}
