using UnityEngine;

public class Splatter : MonoBehaviour
{
    //Splatter
    public GameObject splatHolder;
    public GameObject decal;

    //Particles
    public float randomVelocityFactor; //The spread of the particle -> done
    public float impactForce; //force the particle uses -> done
    public int maxDecals; //MaxDecals that spawn -> done
    public float particleLifeTime; //How long do the particle live -> done
    public float reflectStrength; // impacts the strengt of reflection between hit.normal and the reflect vector3. -> done
    public bool reflectImpact; //if true make the splatter reflect on the opposite angle it impacted -> done
    public GameObject particleToUse; //The particle that spawns when an object is hit -> done
    public float minSplaterSize; //min splatter size -> done
    public float maxSplaterSize; //max splatter size -> done
    public float projectSpawnPosOffset; //How far back the object does the projector need to spawn -> done


    //projectors
    public bool randomYRotation; //use a random y rotation to spawn the projector.

    public bool useNormalSurface; //use the direction of the raycast to spawn the projector or use the normal surface to spawn the projector.
    public float minDecalSize; //this is in the decal script
    public float maxDecalSize; //this is in the decal script

    // Start is called before the first frame update
    void Start()
    {
        splatHolder = GameObject.FindGameObjectWithTag("SplatHolder");
    }

    // Update is called once per frame
    void Update()
    {
        SimulateSplatParticles();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            {
                if (hit.collider.gameObject.isStatic)
                {

                    Vector3 dir;
                    Vector3 spawnPos;

                    if (reflectImpact)
                    {
                        Vector3 reflect = Vector3.Reflect(ray.direction, hit.normal);
                        dir = Vector3.Lerp(hit.normal, reflect, reflectStrength);
                        spawnPos = hit.point + hit.normal * 0.2f;
                    }
                    else
                    {
                        dir = -hit.normal + ray.direction;
                        spawnPos = hit.point + -hit.normal * 0.2f;
                    }
                    
                    SpawnSplatParticles(spawnPos, dir);
                }
            }
        }
    }

    public void SpawnSplatParticles(Vector3 spawnPos, Vector3 direction)
    {
        for (int i = 0; i < maxDecals; i++)
        {
            GameObject splatter = Instantiate(particleToUse, spawnPos, Quaternion.Euler(direction), splatHolder.transform);

            ChangeSize(splatter);

            splatter.GetComponent<SplatParticles>().Init(AddSpread(direction), particleLifeTime, impactForce, decal, useNormalSurface, randomYRotation, spawnPos);
        }
    }

    private void ChangeSize(GameObject splatter)
    {
        float randomSize = Random.Range(minSplaterSize, maxSplaterSize);
        splatter.transform.localScale = splatter.transform.localScale * randomSize;
    }

    private Vector3 AddSpread(Vector3 direction)
    {
        Vector3 newVel = direction;

        newVel *= impactForce;

        Vector3 randomVel = new Vector3(RandomValue(), RandomValue(), RandomValue()).normalized;

        randomVel *= randomVelocityFactor * RandomValue();

        newVel += randomVel;

        return newVel;
    }

    private float RandomValue()
    {
        return Random.Range(-1f, 1f);
    }

    private void SimulateSplatParticles()
    {

    }
}
