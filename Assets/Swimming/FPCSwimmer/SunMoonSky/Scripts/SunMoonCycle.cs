using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunMoonSky
{

    public class SunMoonCycle : MonoBehaviour
    {
        public bool invertRotation = false;
        public float speed = 1f;
        public GameObject sun, moon, moonShape;
        public float sunLightIntensity, moonLightIntensity;
        public GameObject stars;

        private float offset = 0;
        private Light sunLight, moonLight;
        private float sunLightStart, moonLightStart;
        private ParticleSystem starsParticleSys;

        private Material moonMaterial;
        private Material starMaterial;

        void Start()
        {
            if (sun)
            {
                sunLight = sun.GetComponent<Light>();
                if (sunLight)
                {
                    sunLightStart = sunLight.intensity;
                }
            }
            if (moon)
            {
                moonLight = moon.GetComponent<Light>();
                if (moonLight)
                {
                    moonLightStart = moonLight.intensity;
                }
            }
            if (moonShape)
            {
                MeshRenderer r = moonShape.GetComponent<MeshRenderer>();
                moonMaterial = r.material;
            }
            if (stars)
            {
                starsParticleSys = stars.GetComponent<ParticleSystem>();
            }
        }

        void FixedUpdate()
        {            
            offset = speed * Time.fixedDeltaTime;
            this.transform.Rotate(invertRotation? Vector3.right : Vector3.left, offset);

            // Update light intensity
            sunLight.intensity = sunLightIntensity = (transform.rotation.eulerAngles.x >= 180) ? 
                0 : sunLightStart * Mathf.Sin(transform.rotation.eulerAngles.x * Mathf.Deg2Rad);
            moonLight.intensity = moonLightIntensity = (transform.rotation.eulerAngles.x <= 180) ?
                0 : moonLightStart * Mathf.Sin(-transform.rotation.eulerAngles.x * Mathf.Deg2Rad);

            // Update moon fade
            moonMaterial.SetFloat("_Opacity", moonLight.intensity);

            if (starsParticleSys)
            {
                if (moonLightIntensity > 0.2f || (sunLightIntensity < 0.2f && moonLightIntensity != 0))
                {
                    // Show stars
                    starsParticleSys.Play(true);
                }
                else
                {
                    // Stop simulation
                    starsParticleSys.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }
    }

}