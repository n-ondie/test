using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalBehaviours : MonoBehaviour
{
    public float speed = 2f;
    public float boundary = 700f;
    public float initY = -0.7f;
    public int score = 5;

    private SurfGameManager gameManager;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<SurfGameManager>(); 
    }

    // Update is called once per frame
    void Update()
    {
        // Move forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Destroy out of bound
        if (Mathf.Abs(transform.position.x) > boundary || Mathf.Abs(transform.position.z) > boundary){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player collided with the animal
        {
            gameManager.IncrementAnimalCount(); // Increment the animal count
            gameManager.AddScore(score); // Add the animal's score to the player's score

            SurfPlayerController playerController = other.GetComponent<SurfPlayerController>();
            playerController.PlayCollectEffect();
        }

        Destroy(gameObject);
    }
}
