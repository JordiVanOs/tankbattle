using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decalDestroyer : MonoBehaviour
{
    private float decalLifeTime;
    private float decalLifeCounter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        decalLifeCounter += Time.deltaTime;

        if (decalLifeCounter > decalLifeTime)
        {
            DestroyImmediate(gameObject);
        }
    }

    internal void Init(float decalLifeTime)
    {
        this.decalLifeTime = decalLifeTime;
    }
}
