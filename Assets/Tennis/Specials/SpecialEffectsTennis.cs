using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpecialEffectsTennis : MonoBehaviour
{
    public GameObject[] effects;
    float xLimSpawn = 7.5f;
    float ySpawn = 5;
    float zLimSpawnNear = 9.5f;
    float zLimSpawnFar = 20f;

    public float effectsOnScreen = 0;
    float[] spawnIntervals = { 8, 10, 12 };
    float timeToNextSpawn;
    float spawnTimer = 0;

    public Transform ball;
    public Transform player;
    public Transform opponent;

    // Start is called before the first frame update
    void Start()
    {
        SelectSpawnInterval();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer < timeToNextSpawn && effectsOnScreen < 2 && ball.GetComponent<Ball>().inPlay)
        {
            spawnTimer += Time.deltaTime;
        }
        else if (spawnTimer >= timeToNextSpawn)
        {
            SpawnEffect();
            SelectSpawnInterval();
            spawnTimer = 0;
        }
    }

    void SpawnEffect()
    {
        // select the effect randomly
        int randNum = Random.Range(0, effects.Length);
        // spawn at a random position
        Vector3 randSpawnPos = new Vector3(Random.Range(-xLimSpawn, xLimSpawn), ySpawn, Random.Range(zLimSpawnNear, zLimSpawnFar));
        Instantiate(effects[randNum], randSpawnPos, Quaternion.Euler(new Vector3(-90, 0, 0)));
        effectsOnScreen++;
    }

    void SelectSpawnInterval()
    {
        int randNum = Random.Range(0, spawnIntervals.Length);
        timeToNextSpawn = spawnIntervals[randNum];
    }

    public void EnableEffect(string effect)
    {
        if (effect == "Super Speed")
            player.GetComponent<Player>().EnableSuperSpeed();
        else if (effect == "Stuck in the Mud")
            opponent.GetComponent<Opponent>().EnableStuckInTheMud();
    }
}
