using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Navmesh
    NavMeshAgent navMeshAgent;

    // AI params
    public Transform target;
    public float SightRange = 10f;
    private float _distanceToTarget;
    bool isProvoked;
    public float timeBetweenAttacks = 2f;

    // Health & Animator
    EnemyHealth EnemyHealth;
    Animator EnemyAnimator;

    // Damage dealer
    public float damage = 20f;

    // Movement
    public float WalkingSpeed = 1f;

    // Death params
    private float timeInDeathAnim = 1.73f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        navMeshAgent = GetComponent<NavMeshAgent>();
        EnemyHealth = GetComponent<EnemyHealth>();
        EnemyAnimator = GetComponent<Animator>();

        foreach ( Rigidbody rb in GetComponentsInChildren<Rigidbody>() ) rb.isKinematic = true;

        navMeshAgent.speed = WalkingSpeed;
    }

    // Update is called once per frame
    void Update()
    {        
        _distanceToTarget = Vector3.Distance(target.position, transform.position);
        if(isProvoked)
        {
            EngageTarget();
        }
        else if(_distanceToTarget <= SightRange)
        {
            isProvoked = true;
            Debug.Log("within sight");
            // navMeshAgent.SetDestination(target.position);
        }
        
        if(EnemyHealth.IsDead)
        {
            StartCoroutine("Death");
        }
    }

    private void Idle()
    {
        EnemyAnimator.SetTrigger("Idle");
    }

    private void EngageTarget()
    {
        if(_distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
            Debug.Log("Out of attack range");
        }
        if(_distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            AttackTarget();
            Debug.Log("In attack range");
        }
    }

    private void ChaseTarget()
    {
        EnemyAnimator.SetTrigger("Move");
        navMeshAgent.SetDestination(target.position);
    }

    // Animation triggers - consider moving to another file!

    private void AttackTarget()
    {
        EnemyAnimator.ResetTrigger("Move");
        StartCoroutine("AttackAnim", timeBetweenAttacks);

        navMeshAgent.SetDestination(transform.position);

        // if(target == null) return;
        // target.GetComponent<PlayerHealth>().TakeDamage(damage);
        // Debug.Log("Enemy damaged player");
    }

    private IEnumerator AttackAnim(float timeBetweenAttacks)
    {
        RaycastHit hit;

        while(true)
        {
            EnemyAnimator.SetTrigger("Attack - Left");
            yield return new WaitForSeconds(timeBetweenAttacks);
            EnemyAnimator.ResetTrigger("Attack - Left");
            yield return null;

            // EnemyAnimator.SetTrigger("Attack - Right");
            // yield return new WaitForSeconds(timeBetweenAttacks);
            // EnemyAnimator.ResetTrigger("Attack - Right");
        }
    }

    public void AttackHitEvent()
    {
        // if (target = null) return;
        target.GetComponent<PlayerHealth>().TakeDamage(damage);
        Debug.Log("Player health is " + GetComponent<PlayerHealth>().hitPoints);
    }

    private IEnumerator Death()
    {
        while(true)
        {
            Destroy(navMeshAgent);
            EnemyAnimator.SetTrigger("Death");
            yield return new WaitForSeconds(timeInDeathAnim);
            EnemyAnimator.enabled = false;
            foreach ( Rigidbody rb in GetComponentsInChildren<Rigidbody>() ) rb.isKinematic = false;
            Destroy(this);
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Display the enemy's view radius when selected
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }
}
