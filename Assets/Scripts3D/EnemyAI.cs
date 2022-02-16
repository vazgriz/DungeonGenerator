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

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
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
        GetComponent<Animator>().SetTrigger("Move");
        navMeshAgent.SetDestination(target.position);
    }

    private void AttackTarget()
    {
        IsAttacking = true;
        GetComponent<Animator>().ResetTrigger("Move");
        StartCoroutine("AttackType", timeBetweenAttacks);
        Debug.Log(name + " is being attacked by " + target);
    }

    private IEnumerator AttackType(float timeBetweenAttacks)
    {
        while(true)
        {
            GetComponent<Animator>().SetTrigger("Attack - Left");
            yield return new WaitForSeconds(timeBetweenAttacks);
            GetComponent<Animator>().ResetTrigger("Attack - Left");
            GetComponent<Animator>().SetTrigger("Attack - Right");
            yield return new WaitForSeconds(timeBetweenAttacks);
            GetComponent<Animator>().ResetTrigger("Attack - Right");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Display the enemy's view radius when selected
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }
}
