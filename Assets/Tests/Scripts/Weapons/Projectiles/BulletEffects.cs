using System;
using UnityEngine;
using Utilities.Timeline;
using Utilities.Timeline.Events.Point;

namespace Tests.Weapons.Projectiles
{

    [Serializable]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(CapsuleCollider))]
    internal class BulletEffects : MonoBehaviour
    {

        [SerializeField]
        internal ParticleSystem _hitEffect;
        [SerializeField]
        ParticleSystem _projectileEffect;
        [SerializeField]
        LayerMask _mask;
        MeshRenderer _meshRender;
        CapsuleCollider _collider;
        internal Timeline timeline;
        internal Ray shootingRay;
        public void Awake()
        {
            var main = _hitEffect.main;
            main.playOnAwake = false;
            main.stopAction = ParticleSystemStopAction.Disable;
            _meshRender = GetComponent<MeshRenderer>() ?? throw new ComponentCantFindException(gameObject, typeof(MeshRenderer));
            _collider = GetComponent<CapsuleCollider>() ?? throw new ComponentCantFindException(gameObject, typeof(CapsuleCollider));
            var duration = _hitEffect.main.duration;
            timeline = new Timeline(duration);
            timeline.AddPointEvent(1, _ =>
            {
                _hitEffect.transform.SetParent(gameObject.transform);
                _hitEffect.transform.localPosition = Vector3.zero;
                _hitEffect.transform.localRotation = Quaternion.identity;
                _hitEffect.Stop();
                _hitEffect.Clear();
            });
        }
        public void OnEnable()
        {
            _projectileEffect.Play();
            _meshRender.enabled = true;
            _hitEffect.gameObject.SetActive(true);
        }
        public void OnDisable()
        {
            _projectileEffect.Stop();
            _projectileEffect.Clear();
        }
        public void Update()
        {
            timeline.OnUpdate(Time.deltaTime);
        }
        public void OnCollisionStay(Collision collision)
        {
            if (timeline.isRunning)
                return;
            _projectileEffect.Stop();
            _projectileEffect.Clear();
            _meshRender.enabled = false;

            if (Physics.SphereCast(shootingRay, _collider.radius, out var hitInfo, Mathf.Infinity, _mask))
            {
                var point = hitInfo.point;
                var normal = hitInfo.normal;
                _hitEffect.transform.SetParent(null);
                _hitEffect.transform.position = point;
                _hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
                _hitEffect.Play();
                timeline.Restart();
            }

        }
        public void OnCollisionEnter(Collision collision)
        {
            //Debug.Log("effect obj.name: " + collision.gameObject.name);
            //_projectileEffect.Stop();
            //_projectileEffect.Clear();
            //_meshRender.enabled = false;

            //if (Physics.SphereCast(shootingRay, _collider.radius, out var hitInfo, Mathf.Infinity, _mask))
            //{
            //    var point = hitInfo.point;
            //    var normal = hitInfo.normal;
            //    _hitEffect.transform.SetParent(null);
            //    _hitEffect.transform.position = point;
            //    _hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
            //    _hitEffect.Play();
            //    timeline.Restart();
            //}

            //if (Physics.Raycast(shootingRay, out var hitInfo, Mathf.Infinity, _mask))
            //{
            //    var point = hitInfo.point;
            //    var normal = hitInfo.normal;
            //    _hitEffect.transform.SetParent(null);
            //    _hitEffect.transform.position = point;
            //    _hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
            //    _hitEffect.Play();
            //    timeline.Start();
        }

    }
}