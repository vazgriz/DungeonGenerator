using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float hitpoints = 100f;
    // Start is called before the first frame update

    public void TakeDamage(float damage)
    {
        hitpoints -= damage;
        Debug.Log(hitpoints);

        if(hitpoints <=0)
        {
            GetComponent<Animator>().SetTrigger("Death");
            // Destroy(gameObject);
        }
    }
}
