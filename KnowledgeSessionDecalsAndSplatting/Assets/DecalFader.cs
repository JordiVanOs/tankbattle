using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeferredDecal))]
public class DecalFader : MonoBehaviour
{
    private DeferredDecal decal;
    private float decalLifeTime;
    private float decalLifeCounter;
    private float decalStartFadeTime;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        decalLifeCounter += Time.deltaTime;

        float t = (decalLifeCounter - (decalLifeTime - decalStartFadeTime)) / decalStartFadeTime;

        decal.strength = Mathf.Lerp(1.0f, 0.0f, t);

        if (decalLifeCounter > decalLifeTime)
        {
            DestroyImmediate(gameObject);
        }
    }

    internal void Init(float decalLifeTime, float decalStartFadeTime, Material decalMaterial)
    {
        decal = GetComponent<DeferredDecal>();
        decal.m_Material = decalMaterial;
        this.decalLifeTime = decalLifeTime;
        this.decalStartFadeTime = decalStartFadeTime;
    }
}
