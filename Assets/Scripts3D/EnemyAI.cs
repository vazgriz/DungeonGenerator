using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Navmesh
    NavMeshAgent navMeshAgent;

    // AI params
    private Transform target;
    public float SightRange = 5.0f;
    private float _distanceToTarget;
    bool isProvoked = false;
    public float timeBetweenAttacks = 2f;
    private bool IsAttacking = false;

    // Attack params
    // Light attacks
    [SerializeField] float Range = 20f;
    [SerializeField] float damage = 20f;

    // Health & Animator
    EnemyHealth EnemyHealth;
    Animator EnemyAnimator;

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
            // navMeshAgent.SetDestination(target.position);
        }
        
        if(EnemyHealth.IsDead)
        {
            Debug.Log("Time to die");
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
        }
        if(_distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }
    }

    private void ChaseTarget()
    {
        EnemyAnimator.SetTrigger("Move");
        navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        IsAttacking = true;
        EnemyAnimator.ResetTrigger("Move");
        StartCoroutine("AttackType", timeBetweenAttacks);

        if(target == null) return;
        target.GetComponent<PlayerHealth>().TakeDamage(damage);
        Debug.Log("Enemy damaged player");
    }

    private IEnumerator AttackType(float timeBetweenAttacks)
    {
        RaycastHit hit;

        while(true)
        {
            if(Physics.Raycast(transform.position, transform.forward, out hit, Range))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                Debug.Log("enemy is hitting " + hit.transform.name);
            }
            EnemyAnimator.SetTrigger("Attack - Left");
            yield return new WaitForSeconds(timeBetweenAttacks);
            EnemyAnimator.ResetTrigger("Attack - Left");


            // EnemyAnimator.SetTrigger("Attack - Right");
            // yield return new WaitForSeconds(timeBetweenAttacks);
            // EnemyAnimator.ResetTrigger("Attack - Right");
        }
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
