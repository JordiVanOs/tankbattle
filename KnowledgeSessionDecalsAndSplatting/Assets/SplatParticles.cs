using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatParticles : MonoBehaviour
{
    private Rigidbody Rigidbody;
    private Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Rigidbody.AddForce(direction * 10);
    }

    internal void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }
}
