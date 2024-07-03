using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfPlayerController : MonoBehaviour
{
    public float forwardSpeed = 2.5f;
    public float rotationSpeed = 50f;
    public GameObject collectEffectPrefab;

    private Rigidbody rb;
    private Vector3 collectEffectPosOffset = new Vector3(0, 1.4f, 0);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardMovement = transform.forward * forwardSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + forwardMovement);

        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }

    public void PlayCollectEffect()
    {
        Vector3 effectPosition = transform.position + collectEffectPosOffset;
        Instantiate(collectEffectPrefab, effectPosition, Quaternion.identity);
    }
}
