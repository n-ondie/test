using System;
using UnityEngine;



// How to use:
// put a small game object in the scene as a reference point (it will disappear when game starts)
// attach this script to the game object
// drag a prefab to the component (it will be the gem in the game)
public class GemSpawner : MonoBehaviour
{
    public GameObject gemPrefab;
    [SerializeField] private float scale;
    [SerializeField] private Color colour;
    public int numberOfGems = 10;
    public float radius = 5f;   // distance from the center
    
    public float orbitSpeed = 50f;  // Speed of orbit around the center

    private GameObject[] gems;  // Array to hold references to all gem instances
    
    [SerializeField] private bool clockwise = true;
    private float clockwiseValue;

    public float point = 0;
    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();   // disable the mesh renderer in the beginning
        if (renderer != null)
        {
            renderer.enabled = false;
        }
        
        gems = new GameObject[numberOfGems];    // store all gems as one new array
        
        for (int i = 0; i < numberOfGems; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfGems;
            Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            position = transform.rotation * position;
            position += transform.position;
            GameObject gem = Instantiate(gemPrefab, position, Quaternion.identity);
            
            InitializeGemComponents(gem);
            gem.transform.rotation = Quaternion.Euler(-90,0,0); 
            
            gems[i] = gem;  // Store the gem instance (each gem)

            
            // Adjust scale and other properties in the AnimationScript
            AnimationScript animScript = gem.GetComponent<AnimationScript>();
            if (animScript != null)
            {
                animScript.startScale = new Vector3(scale, scale, scale);  // Set start scale
                animScript.endScale = new Vector3(scale, scale, scale);    // Set end scale to be larger
            }
            
            // Modify color
            Renderer gemRenderer = gem.GetComponent<Renderer>();
            if (gemRenderer != null)
            {
                Material newMat = new Material(gemRenderer.material); // Create a new material instance
                newMat.color = colour;
                gemRenderer.material = newMat; // Apply the new material
            }

            gem.AddComponent<Rotator>();
        }
    }

    private void Update()
    {
        if (clockwise == false)
        {
            clockwiseValue = -1;
        } else {
            clockwiseValue = 1;
        }
        
        for (int i = 0; i < numberOfGems; i++)
        {
            GameObject gem = gems[i];
            if (gem != null) // Check if the gem is not null
            {
                float orbitAngle = orbitSpeed * Time.deltaTime * clockwiseValue;
                Vector3 offset = gem.transform.position - transform.position;
                // Create a rotation that is relative to the game object's current rotation
                // Assuming you want to rotate around the local 'up' axis which aligns with typical 'y' axis rotation
                Quaternion rotation = Quaternion.AngleAxis(orbitAngle, transform.up);

                // Apply the rotation to the offset
                offset = rotation * offset;

                // Update the gem position
                gem.transform.position = transform.position + offset;
            }
        }
    }

    private void InitializeGemComponents(GameObject gem)
    {
        // Add the GravityPull script
        GravityPull pullScript = gem.AddComponent<GravityPull>();
        // Set the player transform
        pullScript.player = GameObject.FindWithTag("Player").transform;
        pullScript.point = point;

        // Add Rigidbody and configure it
        Rigidbody gemRigidbody = gem.AddComponent<Rigidbody>();
        gemRigidbody.useGravity = false;

        // Add BoxCollider and configure it as a trigger
        BoxCollider gemCollider = gem.AddComponent<BoxCollider>();
        gemCollider.isTrigger = true;
        
    }
}

public class Rotator : MonoBehaviour
{
    public float selfSpeed = 100f;

    void Update()
    {
        transform.Rotate(0, 0, selfSpeed * Time.deltaTime);
    }
}