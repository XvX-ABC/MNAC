using System;
using UnityEngine;

public class FootCollider : MonoBehaviour
{
    [SerializeField]
    LayerMask _layerMask;
    bool _onGround;
    [SerializeField]
    float _bottomHeight = 0.8f;
    internal Action<RaycastHit> hitAction;
    void FixedUpdate()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, out var hitInfo, _bottomHeight))
        {
            hitAction?.Invoke(hitInfo);
        }
    }

}
