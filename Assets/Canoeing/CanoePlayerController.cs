using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanoePlayerController : MonoBehaviour
{
    public float forwardForce = 10f;
    public float rotationSpeed = 50f;

    private Rigidbody rb;
    public Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            ApplyForwardForce();
            animator.SetTrigger("Row");
        }

        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }

    void ApplyForwardForce()
    {
        Vector3 forceDirection = transform.forward;
        rb.AddForce(forceDirection * forwardForce, ForceMode.Impulse);
    }
}
