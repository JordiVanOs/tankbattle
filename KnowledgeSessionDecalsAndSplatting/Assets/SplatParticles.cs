using UnityEngine;

public class SplatParticles : MonoBehaviour
{
    private Vector3 velocity;
    private float lifeTime;
    private float lifeTimer;
    private float impactForce;
    private GameObject decal;
    private float gravity;
    private bool useNormalSurface;
    private bool randomYRotation;
    private Vector3 hitPositon;
    private float minDecalSize;
    private float maxDecalSize;
    private float decalLifeTime;
    private Transform decalHolder;

    public void Start()
    {
        gravity = 9.807f;
        decalHolder = GameObject.FindGameObjectWithTag("DecalHolder").transform;
    }

    private void Update()
    {
        velocity.y -= gravity * Time.deltaTime;
        
        Ray ray = new Ray(transform.position, velocity.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, velocity.magnitude * Time.deltaTime))
        {
            if (!hit.collider.CompareTag("Splatter"))
            {
                Vector3 dir = Vector3.zero;
                Vector3 spawnPos = hit.point + hit.normal * 0.2f;

                dir = useNormalSurface ? hit.normal : -ray.direction;

                Quaternion rotation = Quaternion.FromToRotation(dir, Vector3.up);
                if (randomYRotation)
                {
                    Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up);
                    rotation *= randomRotation;
                }
                
                GameObject instantiatedDecal = Instantiate(decal, spawnPos, rotation, decalHolder);

                ChangeSize(instantiatedDecal);

                instantiatedDecal.GetComponent<decalDestroyer>().setLifeTime(decalLifeTime);

                Destroy(gameObject);
            }
        }


        transform.Translate(velocity * Time.deltaTime);

        lifeTimer += Time.deltaTime;

        if (lifeTimer > lifeTime)
            DestroyImmediate(gameObject);
    }

    internal void Init(Vector3 velocity, float lifeTime, float impactForce, GameObject decal, bool useNormalSurface, bool randomYRotation, Vector3 hitPositon, float minDecalSize, float maxDecalSize, float decalLifeTime)
    {
        this.velocity = velocity;
        this.lifeTime = lifeTime;
        this.impactForce = impactForce;
        this.decal = decal;
        this.useNormalSurface = useNormalSurface;
        this.randomYRotation = randomYRotation;
        this.hitPositon = hitPositon;
        this.minDecalSize = minDecalSize;
        this.maxDecalSize = maxDecalSize;
        this.decalLifeTime = decalLifeTime;
    }
    
    private void ChangeSize(GameObject decal)
    {
        float randomSizeX = Random.Range(minDecalSize, maxDecalSize);
        float randomSizeZ = Random.Range(minDecalSize, maxDecalSize);

        decal.transform.localScale = new Vector3(randomSizeX, decal.transform.localScale.y, randomSizeZ);
    }
}
