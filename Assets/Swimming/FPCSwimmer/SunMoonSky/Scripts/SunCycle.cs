using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SunMoonSky
{

    public class SunCycle : MonoBehaviour
    {
        public float dayCycleSpeed = 1.0f;

        public bool activeCycle = true;
        public Vector3 rotationIncrement;
        public Vector3 rotationLimit = new Vector3(360, 360, 0);

        public Color sunColor = new Color(255, 255, 255);
        public Color moonColor = new Color(200, 200, 255);

        private Light sun;

        private void Start()
        {
            sun = GetComponent<Light>();
        }

        private void FixedUpdate()
        {
            if (activeCycle)
            {
                transform.eulerAngles += rotationIncrement * Time.deltaTime * dayCycleSpeed;
            }
        }
    }
}