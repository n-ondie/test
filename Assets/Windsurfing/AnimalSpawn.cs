using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSpawn : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    private float spawnRange = 20f;
    private float startDelay = 2f;
    private float spawnInterval = 10f;

    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform.parent;
        InvokeRepeating("SpawnRandomAnimal", startDelay, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnRandomAnimal(){
        // Randomly generate animal index
        int animalIndex = Random.Range(0, animalPrefabs.Length);
        GameObject animalPrefab = animalPrefabs[animalIndex];

        // Get the initial Y position from the AnimalBehaviours script
        AnimalBehaviours animalBehaviour = animalPrefab.GetComponent<AnimalBehaviours>();
        float initY = animalBehaviour.initY;

        // Randomly generate spawn position around the player with the correct Y position
        Vector3 spawnPos = new Vector3(
            Random.Range(-spawnRange, spawnRange),
            initY,
            Random.Range(-spawnRange, spawnRange)
        ) + playerTransform.position;

        // Randomly generate Y rotation
        Quaternion spawnRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        Instantiate(animalPrefab, spawnPos, spawnRotation);
    }
}
