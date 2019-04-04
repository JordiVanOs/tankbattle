using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float rotateSpeed;

    private Transform playWeapon;

    private Transform crossHair;

    // Start is called before the first frame update
    void Start()
    {
        playWeapon = GameObject.FindGameObjectWithTag("PlayerWeapon").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(playWeapon.position, playWeapon.forward);//Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100.0f))
            {
                Splatter splatter = hit.collider.gameObject.GetComponent<Splatter>();
                if (splatter != null)
                {
                    splatter.Splat(ray.direction, hit.normal, hit.point);
                }
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }


        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * (speed / 2) * Time.deltaTime);
        }

        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotateSpeed);
    }
}
