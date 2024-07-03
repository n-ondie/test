using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void AdjustSuspension(WheelCollider wheelCollider) {
        JointSpring spring = wheelCollider.suspensionSpring;
    
        spring.spring = 3000; // Reduced from 8000 for more compression and rebound
        spring.damper = 300; // Lower damping for less absorption and faster rebound
        wheelCollider.suspensionSpring = spring;

        wheelCollider.suspensionDistance = 0.3f; // Increase for more travel distance
    }
}
