using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] Camera FPCamera;
    [SerializeField] float Range = 100f;
    [SerializeField] float damage = 20f;

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, Range))
        {
            Debug.Log(hit.transform.name + " has been shot");
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            target.TakeDamage(damage);
        }
        else
        {
            return;
        }


        Debug.DrawRay(transform.position, FPCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
    }
}
