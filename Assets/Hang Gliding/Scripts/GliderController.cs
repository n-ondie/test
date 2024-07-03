using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderController : MonoBehaviour
{
    public float speed = 12.5f;
    public float drag = 6;

    public Rigidbody rb;

    private Vector3 rot; //rot for rotation

    public float percentage;

    public float turnAngle = 40;
    public float diveAngle = 80;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rot = transform.eulerAngles;
    }

    // Update is called once per frame
    private void Update()
    {
        
        // Rotate the player
        // X (up and down)
        rot.x += 20 * Input.GetAxis("Vertical") * Time.deltaTime;
        rot.x = Mathf.Clamp(rot.x, 0, diveAngle);
        
        // Y (moving left and right)
        rot.y += turnAngle * Input.GetAxis("Horizontal") * Time.deltaTime; // the number is the angle it can turn
        transform.rotation = Quaternion.Euler(rot);
        
        // Z (tilting when turning left and right)
        float targetZ = -10 * Input.GetAxis("Horizontal"); // number is the angle it can tilt
        rot.z = Mathf.Lerp(rot.z, targetZ, 10 * Time.deltaTime);  // Smoothly interpolate towards the target Z
        // rot.z = Mathf.Clamp(rot.z, -5, 5);
        transform.rotation = Quaternion.Euler(rot);

        percentage = rot.x / 45;
        // Drag: Fast(4), Slow(6)
        float modDrag = (percentage * -2) + drag;
        // Speed: Fast(13.8), Slow(12.5)
        float modSpeed = percentage * (13.8f - 12.5f) + speed;
        
        rb.drag = modDrag;
        Vector3 localV = transform.InverseTransformDirection(rb.velocity);
        localV.z = modSpeed;
        rb.velocity = transform.TransformDirection(localV);
        
        // Apply extra gravity force
        if (rot.x > 0) {
            float additionalGravity = (rot.x / 45.0f) * 10f;
            rb.AddForce(Vector3.down * additionalGravity);
        }
    }
}
