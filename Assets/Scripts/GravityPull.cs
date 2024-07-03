using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attach this script to the game object
// drag the player (or the game object that pulls the force) to the component
public class GravityPull : MonoBehaviour
{
    public Transform player;
    private Rigidbody objectRigidbody; // Rigidbody for the game object
    public float influenceRange = 15;
    public float intensity = 50000;
    public float distanceToPlayer;  // For inspecting only
    private Vector3 pullForce;

    public float point;
    
    // Start is called before the first frame update
    void Start()
    {
        objectRigidbody = GetComponent<Rigidbody>(); // Get the Rigidbody of the game object
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer <= influenceRange)
        {
            pullForce = (player.position - transform.position).normalized / distanceToPlayer * intensity;
            objectRigidbody.AddForce(pullForce, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            
        }
    }
}