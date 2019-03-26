using UnityEngine;

public class Splatter : MonoBehaviour
{
    public GameObject decal;

    public float randomVelocityFactor; //The spread the particle

    public float impactForce; //force the particle uses -> done

    public int maxDecals; //MaxDecals that spawn -> done

    public float particleLifeTime; //How long do the particle live -> done


    public bool randomRotation; //random rotation over y as.

    public bool useNormalSurface; //if true use normal surface if false use the direction it collided


    public GameObject particleToUse; //The particle that spawns when an object is hit -> done

    public float minSplaterSize; //min splatter size -> done
    public float maxSplaterSize; //max splatter size -> done


    public float projectSpawnPosOffset; //How far back the object does the projector need to spawn -> done

    public float gravity; //For rigidbody

    public float minDecalSize; //this is in the decal script
    public float maxDecalSize; //this is in the decal script

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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            {
                if (hit.collider.gameObject.isStatic)
                {
                    Instantiate(decal, hit.point + hit.normal * projectSpawnPosOffset, Quaternion.FromToRotation(Vector3.up, hit.normal));
                    Vector3 reflect = Vector3.Reflect(ray.direction, hit.normal);
                    SpawnSplatParticles(hit.point, reflect, 10, 10);
                }
            }
        }
    }

    public void SpawnSplatParticles(Vector3 spawnPos, Vector3 direction, int strength, int count)
    {
        for (int i = 0; i < maxDecals; i++)
        {
            Vector3 newVel = direction;

            newVel *= impactForce;
            
            Vector3 randomVel = new Vector3(RandomValue(), RandomValue(), RandomValue()).normalized;

            randomVel *= randomVelocityFactor;

            newVel += randomVel;
            
            GameObject splatter = Instantiate(particleToUse, spawnPos, Quaternion.Euler(direction));

            float randomSize = Random.Range(minSplaterSize, maxSplaterSize);
            splatter.transform.localScale = splatter.transform.localScale * randomSize;

            splatter.transform.Rotate(newVel);


            splatter.GetComponent<SplatParticles>().Init(newVel, particleLifeTime, impactForce);
        }
    }

    private float RandomValue()
    {
        return Random.Range(-1, 1);
    }

    private void SimulateSplatParticles()
    {

    }
}
