using UnityEngine;

public class Splatter : MonoBehaviour
{
    public GameObject decal;
    public Vector3 spreadRange;
    public float impactForce;
    public float gravity;
    public int maxDecals;
    public float particleLifeTime;
    public float minDecalSize;
    public float maxDecalSize;
    public bool randomRotation;
    public bool useNormalSurfaceNormal;
    public GameObject particleToUse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SimulateSplatParticles();

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.isStatic)
                {
                    Vector3 reflect = Vector3.Reflect(ray.direction, hit.normal);
                    SpawnSplatParticles(hit.point, reflect, 10, 10);
                }
            }
        }
    }

    public void SpawnSplatParticles(Vector3 spawnPos, Vector3 direction, int strength, int count)
    {
        SplatParticles splatter = Instantiate(particleToUse, spawnPos, Quaternion.Euler(direction)).GetComponent<SplatParticles>();
        splatter.SetDirection(direction.normalized);
    }

    public void SetSize()
    {
        float sizeMod = Random.Range(minDecalSize, maxDecalSize);
        transform.localScale *= sizeMod;
    }

    public void SetRotation()
    {
        float randomRotationZ = Random.Range(-360, 360);
        float randomRotationX = Random.Range(-360, 360);
        float randomRotationY = Random.Range(-360, 360);

        transform.rotation = Quaternion.Euler(randomRotationX, randomRotationY, randomRotationZ);
    }

    private void SimulateSplatParticles()
    {

    }
}
