using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Navmesh
    private NavMeshAgent _navMeshAgent;

    // AI params
    public Transform Target;
    public float SightRange = 10f;
    public float TimeBetweenAttacks = 2f;
    private float _distanceToTarget;
    private bool _isProvoked;

    // Health & Animator
    private EnemyHealth _enemyHealth;
    private Animator _enemyAnimator;

    // Damage dealer
    public float Damage = 20f;

    // Movement
    public float WalkingSpeed = 1f;

    // Death params
    private float _timeInDeathAnim = 1.73f;
    private float _timeUntilDeadEnemyDisappears = 3f;

    // Start is called before the first frame update
    void Start()
    {
        Target = GameObject.FindGameObjectWithTag("Player").transform;

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyAnimator = GetComponent<Animator>();

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
        }

        _navMeshAgent.speed = WalkingSpeed;
    }

    // Update is called once per frame
    void Update()
    {        
        _distanceToTarget = Vector3.Distance(Target.position, transform.position);
        if (_isProvoked)
        {
            EngageTarget();
        }
        else if (_distanceToTarget <= SightRange)
        {
            _isProvoked = true;
            Debug.Log("within sight");
        }
        
        if (_enemyHealth.IsDead)
        {
            StartCoroutine("Death");
        }
    }

    private void Idle()
    {
        _enemyAnimator.SetTrigger("Idle");
    }

    private void EngageTarget()
    {
        if (_distanceToTarget >= _navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
            Debug.Log("Out of attack range");
        }
        if (_distanceToTarget <= _navMeshAgent.stoppingDistance)
        {
            AttackTarget();
            Debug.Log("In attack range");
        }
    }

    private void ChaseTarget()
    {
        _enemyAnimator.SetTrigger("Move");
        if (Target != null)
        {
            _navMeshAgent.SetDestination(Target.position);
        }
    }

    // Animation triggers - consider moving to another file!

    private void AttackTarget()
    {
        _enemyAnimator.ResetTrigger("Move");
        StartCoroutine("AttackAnim", TimeBetweenAttacks);
        if (Target != null)
        {
            _navMeshAgent.SetDestination(transform.position);
        }
    }

    private IEnumerator AttackAnim(float timeBetweenAttacks)
    {
        RaycastHit hit;

        while(true)
        {
            _enemyAnimator.SetTrigger("Attack - Left");
            yield return new WaitForSeconds(timeBetweenAttacks);
            _enemyAnimator.ResetTrigger("Attack - Left");
            yield return null;
        }
    }

    public void AttackHitEvent()
    {
        Debug.Log(Target);
        Target.GetComponent<PlayerHealth>().TakeDamage(Damage);
    }

    private IEnumerator Death()
    {
        while (true)
        {
            _navMeshAgent.enabled = false;
            _enemyAnimator.SetTrigger("Death");
            yield return new WaitForSeconds(_timeInDeathAnim);

            _enemyAnimator.enabled = false;

            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
            }

            yield return new WaitForSeconds(_timeUntilDeadEnemyDisappears);

            Destroy(gameObject);
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
