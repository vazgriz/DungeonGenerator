using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Transform target;
    NavMeshAgent navMeshAgent;

    public float SightRange = 5.0f;
    private float _distanceToTarget;
    bool isProvoked = false;
    public float WalkingSpeed = 1f;

    public float timeBetweenAttacks = 2f;
    private bool IsAttacking = false;

    private float timeInDeathAnim = 1.73f;

    EnemyHealth EnemyHealth;
    Animator EnemyAnimator;

    // Start is called before the first frame update
    void Start()
    {
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
    }

    private IEnumerator AttackType(float timeBetweenAttacks)
    {
        while(true)
        {
            EnemyAnimator.SetTrigger("Attack - Left");
            yield return new WaitForSeconds(timeBetweenAttacks);
            EnemyAnimator.ResetTrigger("Attack - Left");
            EnemyAnimator.SetTrigger("Attack - Right");
            yield return new WaitForSeconds(timeBetweenAttacks);
            EnemyAnimator.ResetTrigger("Attack - Right");
        }
    }

    private IEnumerator Death()
    {
        while(true)
        {
            Destroy(navMeshAgent);
            Debug.Log("death started");
            EnemyAnimator.SetTrigger("Death");
            yield return new WaitForSeconds(timeInDeathAnim);
            EnemyAnimator.enabled = false;
            Debug.Log("animator turned off");
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
