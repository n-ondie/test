using Crest;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FPCSwimmer
{

    public class ReflectionsToOceanMaterial : MonoBehaviour
    {
        public GameObject oceanObject;
        public Material oceanMaterial;
        private OceanRenderer oceanRenderer;

        void Awake()
        {
            if (oceanObject)
            {
                oceanRenderer = oceanObject.GetComponent<OceanRenderer>();
                if (oceanRenderer)
                {
                    oceanMaterial = oceanRenderer.OceanMaterial;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            ReflectionProbe rp = GetComponent<ReflectionProbe>();
            // Override ocean material reflection map with the reflections of this probe.
            oceanMaterial.SetFloat("_OverrideReflectionCubemap", 1);
            oceanMaterial.SetTexture("_ReflectionCubemapOverride", rp.texture);
        }
    }

}
