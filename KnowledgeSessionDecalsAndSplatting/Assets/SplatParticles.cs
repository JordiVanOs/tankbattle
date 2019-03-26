using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatParticles : MonoBehaviour
{
    private Rigidbody Rigidbody;
    private Vector3 velocity;
    private float lifeTime;
    private float lifeTimer;
    private float impactForce;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       Rigidbody.AddForce(velocity);
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer > lifeTime)
            DestroyImmediate(gameObject);
    }

    internal void Init(Vector3 velocity, float lifeTime, float impactForce)
    {
        this.velocity = velocity;
        this.lifeTime = lifeTime;
        this.impactForce = impactForce;
    }
}
