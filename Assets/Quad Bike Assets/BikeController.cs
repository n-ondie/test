using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BikeController : MonoBehaviour
{
    private Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    public WheelParticles wheelParticles;
    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    public GameObject smokePrefab;
    public float motorPower;
    public float brakePower;
    private float slipAngle;
    public float speed;
    public AnimationCurve steeringCurve;
    
    // Start is called before the first frame update
    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
        InstantiateSmoke();
    }

    void InstantiateSmoke()
    {
        wheelParticles.FLWheel = Instantiate(smokePrefab, colliders.FLWheel.transform.position - Vector3.up * colliders.FLWheel.radius, Quaternion.identity, colliders.FLWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.FRWheel = Instantiate(smokePrefab, colliders.FRWheel.transform.position - Vector3.up * colliders.FRWheel.radius, Quaternion.identity, colliders.FRWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.RLWheel = Instantiate(smokePrefab, colliders.RLWheel.transform.position - Vector3.up * colliders.RLWheel.radius, Quaternion.identity, colliders.RLWheel.transform)
            .GetComponent<ParticleSystem>();
        wheelParticles.RRWheel = Instantiate(smokePrefab, colliders.RRWheel.transform.position - Vector3.up * colliders.RRWheel.radius, Quaternion.identity, colliders.RRWheel.transform)
            .GetComponent<ParticleSystem>();
    
    }
    
    //Update is called once per frame
    void Update()
    {
        speed = playerRB.velocity.magnitude;
        CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();  
        ApplyWheelPositions();
        CheckParticles();
    }

    void CheckInput()
    {
        // gasInput = Input.GetAxis("Vertical");
        // if (gasPedal.isPressed)
        // {
        //     gasInput += gasPedal.dampenPress;
        // }
        // if (brakePedal.isPressed)
        // {
        //     gasInput -= brakePedal.dampenPress;
        // }
        // steeringInput = Input.GetAxis("Horizontal");
        // if (rightButton.isPressed)
        // {
        //     steeringInput += rightButton.dampenPress;
        // }
        // if (leftButton.isPressed)
        // {
        //     steeringInput -= leftButton.dampenPress;
        // }
        // slipAngle = Vector3.Angle(transform.forward, playerRB.velocity-transform.forward);
        //
        // //fixed code to brake even after going on reverse by Andrew Alex 
        // float movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);
        // if (movingDirection < -0.5f && gasInput > 0)
        // {
        //     brakeInput = Mathf.Abs(gasInput);
        // }
        // else if (movingDirection > 0.5f && gasInput < 0)
        // {
        //     brakeInput = Mathf.Abs(gasInput);
        // }
        // else
        // {
        //     brakeInput = 0;
        // }
        
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
        slipAngle = Vector3.Angle(transform.forward, playerRB.velocity-transform.forward);
        if (slipAngle < 150f)
        {
            if (gasInput < 0)
            {
                brakeInput = Mathf.Abs(gasInput);
                gasInput = 0;
            }
            else
            {
                brakeInput = 0;
            }
        }
        else
        {
            brakeInput = 0;
        }
    }

    void ApplyBrake()
    {
        colliders.FLWheel.brakeTorque = brakeInput * brakePower*0.7f;
        colliders.FRWheel.brakeTorque = brakeInput * brakePower*0.7f;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower*0.3f;
        colliders.RRWheel.brakeTorque = brakeInput * brakePower*0.3f;

    }

    void ApplyMotor()
    {
        colliders.RRWheel.motorTorque = motorPower * gasInput*3f;
        colliders.RLWheel.motorTorque = motorPower * gasInput*3f;
    }

    void ApplySteering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);
        colliders.FLWheel.steerAngle = steeringAngle;
        colliders.FRWheel.steerAngle = steeringAngle;
    }
    void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);

    }

    void CheckParticles()
    {
        WheelHit[] wheelHits = new WheelHit[4];
        colliders.FLWheel.GetGroundHit(out wheelHits[0]);
        colliders.FRWheel.GetGroundHit(out wheelHits[1]);
        colliders.RLWheel.GetGroundHit(out wheelHits[2]);
        colliders.RRWheel.GetGroundHit(out wheelHits[3]);

        float slipAllowance = 0.3f;
        if (Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance)
        {
            wheelParticles.FLWheel.Play();
        }
        else
        {
            wheelParticles.FLWheel.Stop();
        }
        if (Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance)
        {
            wheelParticles.FRWheel.Play();
        }
        else
        {
            wheelParticles.FRWheel.Stop();
        }
        if (Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance)
        {
            wheelParticles.RLWheel.Play();
        }
        else
        {
            wheelParticles.RLWheel.Stop();
        }
        if (Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance)
        {
            wheelParticles.RRWheel.Play();
        }
        else
        {
            wheelParticles.RRWheel.Stop();
        }
    }
    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }
}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FLWheel;
    public WheelCollider FRWheel;
    public WheelCollider RLWheel;
    public WheelCollider RRWheel;
    
}
[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FLWheel;
    public MeshRenderer FRWheel;
    public MeshRenderer RLWheel;
    public MeshRenderer RRWheel;
}

[System.Serializable]
public class WheelParticles
{
    public ParticleSystem FLWheel;
    public ParticleSystem FRWheel;
    public ParticleSystem RLWheel;
    public ParticleSystem RRWheel;
}