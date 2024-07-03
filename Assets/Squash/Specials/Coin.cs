using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    float timer = 0;
    float lifespan;
    float[] availTimes = { 8, 9, 10, 11, 12 };
    public int pointValue;
    public AudioClip coinCollect;

    //SpecialEffectsSquash effectManager;
    LogicSquash scoreManager;
    public GameObject floatingText;

    // Start is called before the first frame update
    void Start()
    {
        //effectManager = GameObject.FindGameObjectWithTag("Special Effects").GetComponent<SpecialEffectsSquash>();
        scoreManager = GameObject.Find("Logic Squash").GetComponent<LogicSquash>();

        // coin appears for a random length of time
        int randNum = Random.Range(0, availTimes.Length);
        lifespan = availTimes[randNum];
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < lifespan && !scoreManager.gameOver)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) // 'other' is the object that has collided with the trigger
    {
        if (other.CompareTag("Player"))
        {
            scoreManager.AddScore(pointValue, true);
            AudioSource.PlayClipAtPoint(coinCollect, transform.position, 0.6f);
            Destroy(gameObject);

            // show floating text indicating time gained
            GameObject pointsInstance = Instantiate(floatingText, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            if (scoreManager.doublePoints)
                pointsInstance.transform.GetChild(0).GetComponent<TMP_Text>().text = "+" + 2*pointValue;
            else
                pointsInstance.transform.GetChild(0).GetComponent<TMP_Text>().text = "+" + pointValue;
            pointsInstance.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.green;
        }
    }
}
