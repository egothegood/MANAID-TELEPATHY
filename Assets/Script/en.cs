using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class en : MonoBehaviour
{
    bool isAlive = true;
    int hitcount = 0;

    void OnCollisionEnter(Collision collision)
    {

        if (isAlive && collision.gameObject.CompareTag("telepathy"))
        {
            ++hitcount;

        }
    }

    private void Update()
    {
        if (hitcount == 3)
        {
            Die();
        }
    }

    void Die()
    {
        // Disable the enemy object
        gameObject.SetActive(false);
        // Alternatively, you can destroy the enemy object if you don't need it anymore
        // Destroy(gameObject);

        isAlive = false;
    }
}
