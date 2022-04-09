using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitpoints = 100f;
    public bool IsDead = false;
    // Start is called before the first frame update

    public void TakeDamage(float damage)
    {
        hitpoints -= damage;
        Debug.Log(hitpoints);

        if(hitpoints <=0)
        {
            IsDead = true;
        }
    }
}
