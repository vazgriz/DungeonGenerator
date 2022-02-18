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

    public float timeBetweenAttacks = 2f;
    private bool IsAttacking = false;

    public float timeInDeathAnim = 2f;

    EnemyHealth EnemyHealth;
    Animator EnemyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        EnemyHealth = GetComponent<EnemyHealth>();
        EnemyAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _distanceToTarget = Vector3.Distance(target.position, transform.position);
        if(isProvoked)
        {
            EngageTarget();

            Debug.Log("Engaging ", target);
        }
        else if(_distanceToTarget <= SightRange)
        {
            isProvoked = true;
            // navMeshAgent.SetDestination(target.position);
        }
        
        if(EnemyHealth.IsDead)
        {
            StartCoroutine("Death", timeInDeathAnim);
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
            Debug.Log(name + " is within stopping distance");
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
        Debug.Log(name + " is being attacked by " + target);
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

    private IEnumerator Death(float timeInDeathAnim)
    {
        while(true)
        {
            EnemyAnimator.SetTrigger("Death");
            // Destroy(navMeshAgent);
            yield return new WaitForSeconds(timeInDeathAnim);
            EnemyAnimator.enabled = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Display the enemy's view radius when selected
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }
}
