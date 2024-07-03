using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRunAway : MonoBehaviour
{
    float speed = 6f;
    float xBoundary = 40f;
    Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        // find z value of position on x-boundary that the animal should move towards
        float yRotRad = transform.eulerAngles.y * Mathf.PI / 180;
        if (transform.rotation.y > 0)
            targetPos = new Vector3(xBoundary, 50, transform.position.z + 40 / Mathf.Tan(yRotRad));
        else
            targetPos = new Vector3(-xBoundary, 50, transform.position.z + 40 / Mathf.Tan(-yRotRad));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Destroy once boundary is reached
        if (Mathf.Abs(transform.position.x) >= xBoundary)
        {
            Destroy(gameObject);
        }
    }
}
