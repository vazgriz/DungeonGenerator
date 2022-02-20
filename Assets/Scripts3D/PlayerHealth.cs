using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float hitPoints = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        Debug.Log("Player health is " + hitPoints);
        if (hitPoints <= 0)
        {
            Debug.Log("Player is dead");
        }
    }
}
