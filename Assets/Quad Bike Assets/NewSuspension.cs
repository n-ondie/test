using UnityEngine;

public class WheelBehavior : MonoBehaviour
{
    public WheelCollider wheelCollider;
    public float springStrength = 20000f;
    public float springDamper = 1500f;
    
    void FixedUpdate()
    {
        ApplySuspension();
    }

    void ApplySuspension()
    {
        RaycastHit hit;
        if (Physics.Raycast(wheelCollider.transform.position, -transform.up, out hit, wheelCollider.suspensionDistance + wheelCollider.radius))
        {
            Vector3 springDirection = transform.up;
            Vector3 tireVelocity = GetComponentInParent<Rigidbody>().GetPointVelocity(wheelCollider.transform.position);
            float offset = wheelCollider.suspensionDistance - hit.distance;
            float velocity = Vector3.Dot(springDirection, tireVelocity);
            float force = (offset * springStrength) - (velocity * springDamper);
            GetComponentInParent<Rigidbody>().AddForceAtPosition(springDirection * force, wheelCollider.transform.position);
        }
    }
}