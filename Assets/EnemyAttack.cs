using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Attack params
    // Light attacks
    [SerializeField] float Range = 20f;
    [SerializeField] float damage = 40f;

    // Target
    EnemyAI enemyAI;
    private Transform playerTarget;

    // Start is called before the first frame update
    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        playerTarget = enemyAI.target;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public void AttackHitEvent()
    // {
    //     if (playerTarget = null) return;
    //     playerTarget.GetComponent<PlayerHealth>().TakeDamage(damage);
    //     Debug.Log("Player damaged");
    // }
}

            // if(Physics.Raycast(transform.position, transform.forward, out hit, Range))
            // {
            //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //     // Debug.Log("enemy is hitting " + hit.transform.name);
            // }