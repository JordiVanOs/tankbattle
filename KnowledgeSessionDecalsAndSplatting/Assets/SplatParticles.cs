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

    public void Start()
    {
        gravity = 9.807f;
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
                Vector3 spawnPos = hit.point + hit.normal * 0.1f;

                if (useNormalSurface)
                {
                    dir = hit.normal;
                }
                else
                {
                    dir = hit.normal + ray.direction;
                }

                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, dir);
                if (randomYRotation)
                {
                    float randomY = Random.Range(-360.0f, 360.0f);
                    rotation = Quaternion.Euler(rotation.eulerAngles.x, randomY, rotation.eulerAngles.y);
                }

                Instantiate(decal, spawnPos, rotation);

                Destroy(gameObject);
            }
        }


        transform.Translate(velocity * Time.deltaTime);

        lifeTimer += Time.deltaTime;

        if (lifeTimer > lifeTime)
            DestroyImmediate(gameObject);
    }

    internal void Init(Vector3 velocity, float lifeTime, float impactForce, GameObject decal, bool useNormalSurface, bool randomYRotation)
    {
        this.velocity = velocity;
        this.lifeTime = lifeTime;
        this.impactForce = impactForce;
        this.decal = decal;
        this.useNormalSurface = useNormalSurface;
        this.randomYRotation = randomYRotation;
    }

    public void OnCollisionEnter(Collision other)
    {
    }
}
