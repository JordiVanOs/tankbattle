using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ParticleScribtableObject : ScriptableObject
{
    public float lifeTime;
    public Transform decalHolder;
    public float minDecalSize;
    public float maxDecalSize;
    public GameObject decal;
}
