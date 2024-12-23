using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [SerializeField]
    GameObject _followingTarget;
    Vector3 _oldPosition;
    void Start()
    {
        _oldPosition = _followingTarget.transform.position;
        this.transform.LookAt(_followingTarget.transform);
    }
    public void UpdatePos()
    {
        var currentPos = _followingTarget.transform.position;
        var df = currentPos - _oldPosition;
        this.transform.position += df;
        _oldPosition = currentPos;
    }
    public void UpdatePos(Vector3 currentPos)
    {
        var df = currentPos - _oldPosition;
        this.transform.position += df;
        _oldPosition = currentPos;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        UpdatePos();
    }
}
