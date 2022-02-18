using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] Camera FPCamera;
    [SerializeField] float Range = 100f;
    [SerializeField] float damage = 20f;
    [SerializeField] ParticleSystem MuzzleFlash;

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            PlayMuzzleFlash();
            Shoot();
        }
    }

    public void PlayMuzzleFlash()
    {
        MuzzleFlash.Play();
        Debug.Log("Muzzle Flash");
    }

    private void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(FPCamera.transform.position, FPCamera.transform.forward, out hit, Range))
        {
            Debug.Log(hit.transform.name + " has been shot");
            Debug.DrawRay(transform.position, FPCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            EnemyHealth target = hit.transform.GetComponentInParent<EnemyHealth>();
            if(target == null)
            {
                return;
            }
            
            target.TakeDamage(damage);
        }
        else
        {
            return;
        }


    }
}
