using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    bool isAlive = true;
    int hitcount = 0;
    public float knockbackForce = 5f;
    private Rigidbody rb;
    public float moveSpeed = 2f;
    public float moveInterval = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        InvokeRepeating("RandomMove", 0f, moveInterval); // Start periodic random movement
    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (isAlive && collision.gameObject.CompareTag("telepathy"))
        {
            Vector3 knockbackDirection = transform.position - collision.contacts[0].point;
            rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
            ++hitcount;
            Debug.Log(hitcount);

        }
    }

    private void Update()
    {
        if (hitcount >= 3)
        {
            Die();
        }
    }

    void RandomMove()
    {
        // Generate a random direction
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere;
        randomDirection.y = 0; // Ensure the enemy moves only on the XZ plane (optional)

        // Move the enemy in the random direction
        rb.MovePosition(rb.position + randomDirection.normalized * moveSpeed * Time.deltaTime);
    }

    void Die()
    {
        
        //gameObject.SetActive(false);
       
        Destroy(gameObject);

        isAlive = false;
    }
}
