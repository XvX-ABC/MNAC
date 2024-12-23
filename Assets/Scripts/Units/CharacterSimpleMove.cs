using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSimpleMove : MonoBehaviour
{
    Rigidbody _body;
    void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _body.rotation = Quaternion.LookRotation(Vector3.forward);
    }
    void FixedUpdate()
    {

    }
}
