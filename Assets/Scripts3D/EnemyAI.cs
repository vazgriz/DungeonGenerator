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

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        _distanceToTarget = Vector3.Distance(target.position, transform.position);

        if(_distanceToTarget <= SightRange)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Display the enemy's view radius when selected
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }
}
