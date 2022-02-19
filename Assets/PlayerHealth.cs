using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public float Health = 100f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeDamage()
    {

    }

    void Death()
    {
        if (Health <= 0)
        {
            Debug.Log("Player is dead");
        }
    }
}
