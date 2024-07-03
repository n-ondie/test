using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    // Camera shake is only needed for first person.
    public GliderController gc;

    public float shaking = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        float mod_shaking = shaking * gc.percentage;
        transform.localPosition = new Vector3(Random.Range(-shaking, shaking), Random.Range(-shaking, shaking), 0);

    }
}
