using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace Tests.Weapons
{
    public class Gun : MonoBehaviour, ILauncher
    {
        ObjectPool<GameObject> _bulletsPool;
        ILauncherDefines _defines;
        [SerializeField]
        float _fireInterval;
        [SerializeField]
        float _magazineRemainingCount;
        [SerializeField]
        float _remainingCount;
        float _lastFireTime;
        float _lastReloadTime;

        public float RemainingCount { get => _remainingCount; }
        public float MagazineRemaingCount { get => _magazineRemainingCount; set => _magazineRemainingCount = value; }
        void Awake()
        {
            _defines = GetComponent<ILauncherDefines>() ?? throw new ComponentCantFoundException(this.gameObject, typeof(ILauncherDefines));
        }
        void OnEnable()
        {
            _bulletsPool = new(
                () =>
                {
                    var origin = _defines.ProjectileOrigin;
                    var obj = Instantiate(origin, this.transform);
                    obj.name = origin.name + "_" + _bulletsPool.CountAll;
                    obj.transform.localPosition = _defines.BorePosition;
                    obj.SetActive(false);


                    if (obj.TryGetComponent<IProjectile>(out var p))
                        p.HitAction += ReleaseBullet;
                    return obj;
                },
                obj =>
                {
                    //obj.transform.localPosition = _defines.MuzzlePosition;
                    obj.transform.position = this.transform.position + this.transform.rotation * _defines.MuzzlePosition;
                    obj.transform.SetParent(null);
                    obj.SetActive(true);
                },
                obj =>
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(this.transform);
                    obj.transform.localPosition = _defines.BorePosition;
                },
                obj =>
                {
                    if (obj.TryGetComponent<IProjectile>(out var p))
                        p.HitAction -= ReleaseBullet;
                }
                );
        }
        void OnDisable()
        {
            _bulletsPool.Dispose();
        }

        void Start()
        {
            if (_defines.FiringRate > 0)
                _fireInterval = 1 / _defines.FiringRate;
            else
                this.enabled = false;
            _magazineRemainingCount = _defines.BulletsTotalNumInMagazine;
            _remainingCount = Mathf.Max(0, _defines.BulletsTotalNum - _defines.BulletsTotalNumInMagazine);
        }
        void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
                Fire();
            if (Input.GetKeyDown(KeyCode.R))
                Reload();
            if (Input.GetKeyDown(KeyCode.Space))
                Suppelement(10);
        }
        protected virtual void ReleaseBullet(IProjectile projectile, GameObject hitObj)
        {
            if (hitObj == this.gameObject)
                return;
            _bulletsPool.Release(projectile.Object);
        }
        public void Suppelement(ushort num)
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled)
                return;
            _remainingCount = Mathf.Min(_defines.BulletsTotalNum - _defines.BulletsTotalNumInMagazine, _remainingCount + num);
        }
        public bool Reload()
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled)
                return false;
            if (Time.time - _lastReloadTime <= _defines.ReloadDuration || _remainingCount <= 0)
                return false;

            var num = Mathf.Min(_remainingCount, _defines.BulletsTotalNumInMagazine - _magazineRemainingCount);
            _magazineRemainingCount += num;
            _remainingCount -= num;
            _lastReloadTime = Time.time;
            return true;
        }
        public void Fire()
        {
            Debug.Log(MethodInfo.GetCurrentMethod().Name);
            if (!enabled)
                return;
            var time = Time.time;
            if (time - _lastFireTime <= _fireInterval || _magazineRemainingCount <= 0)
                return;
            var obj = _bulletsPool.Get();
            _magazineRemainingCount--;
            _lastFireTime = Time.time;
        }

    }
}