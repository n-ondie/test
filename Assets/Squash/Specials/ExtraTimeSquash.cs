using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExtraTimeSquash : MonoBehaviour
{
    float timer = 0;
    float lifespan;
    float[] availTimes = { 6, 8, 10, 12, 14 };

    SpecialEffectsSquash effectManager;
    LogicSquash scoreManager;
    public GameObject floatingText;

    // Start is called before the first frame update
    void Start()
    {
        effectManager = GameObject.FindGameObjectWithTag("Special Effects").GetComponent<SpecialEffectsSquash>();
        scoreManager = GameObject.Find("Logic Squash").GetComponent<LogicSquash>();

        // effect appears for a random length of time
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
            Disappear();
        }
    }

    void Disappear()
    {
        effectManager.effectsOnScreen--;
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) // 'other' is the object that has collided with the trigger
    {
        if (other.CompareTag("Player"))
        {
            effectManager.EnableEffect("Extra Time");
            Disappear();

            // show floating text indicating time gained
            GameObject pointsInstance = Instantiate(floatingText, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
            pointsInstance.transform.GetChild(0).GetComponent<TMP_Text>().text = "+10s";
            pointsInstance.transform.GetChild(0).GetComponent<TMP_Text>().color = Color.magenta;
        }
    }
}
