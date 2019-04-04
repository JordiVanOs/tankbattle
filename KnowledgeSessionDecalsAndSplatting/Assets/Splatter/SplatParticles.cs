using UnityEngine;

public class SplatParticles : MonoBehaviour
{
    private Vector3 velocity;
    private float lifeTimer;
    private float impactForce;
    private float gravity;
    private GameObject decalHolder;
    private bool useNormalSurface;
    private bool randomYRotation;
    private Vector3 hitPositon;
    private float minDecalSize;
    private float maxDecalSize;
    private float decalLifeTime;
    private GameObject decal;
    private float lifeTime;
    private Material decalMaterial;
    private float decalStartFadeTime;

    private static Quaternion rotate90 = Quaternion.Euler(-90, 0, 0);

    public void Start()
    {
        gravity = 9.807f;
        decalHolder = GameObject.FindGameObjectWithTag("DecalHolder");

        if (decalHolder == null)
        {
            decalHolder = new GameObject("DecalHolder");
            decalHolder.tag = "DecalHolder";
        }
    }

    private void Update()
    {
        velocity.y -= gravity * Time.deltaTime;
        
        Ray ray = new Ray(transform.position, velocity.normalized);

        if (Physics.Raycast(ray, out RaycastHit hit, velocity.magnitude * Time.deltaTime))
        {
            if (!hit.collider.CompareTag("Splatter"))
            {
                if (hit.collider.gameObject.isStatic)
                {
                    Vector3 dir = useNormalSurface ? -hit.normal : ray.direction;
                    Vector3 spawnPos = hit.point + hit.normal * 0.1f;

                    //Quaternion rotation = Quaternion.FromToRotation(dir, Vector3.up);
                    Quaternion rotation = Quaternion.LookRotation(dir, new Vector3(0, 1, 0));
                    rotation *= rotate90;

                    if (randomYRotation)
                    {
                        Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up);
                        rotation *= randomRotation;
                    }
                    

                    GameObject instantiatedDecal = Instantiate(decal, spawnPos, rotation, decalHolder.transform);

                    ChangeSize(instantiatedDecal);

                    //TODO remove decaldestroyer script and add the functionality to the decal script.
                    instantiatedDecal.GetComponent<DecalFader>().Init(decalLifeTime, decalStartFadeTime, decalMaterial);

                    Destroy(gameObject);
                }
            }
        }


        transform.Translate(velocity * Time.deltaTime);

        lifeTimer += Time.deltaTime;

        if (lifeTimer > lifeTime)
            DestroyImmediate(gameObject);
    }

    internal void Init(Vector3 velocity, float lifeTime, float impactForce, GameObject decal, bool useNormalSurface, bool randomYRotation, Vector3 hitPositon, float minDecalSize, float maxDecalSize, float decalLifeTime, Material decalMaterial, float decalStartFadeTime)
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
        this.decalMaterial = decalMaterial;
        this.decalStartFadeTime = decalStartFadeTime;
    }
    
    private void ChangeSize(GameObject decal)
    {
        float randomSize = Random.Range(minDecalSize, maxDecalSize);

        decal.transform.localScale = decal.transform.localScale * randomSize;
    }
}
