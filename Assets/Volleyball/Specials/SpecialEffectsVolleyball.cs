using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffectsVolleyball : MonoBehaviour
{
    public GameObject[] effects;
    public GameObject[] coins;
    float xLimSpawn = 9f;
    float ySpawn = 5.5f;
    float zLimSpawnNear = -14f;
    float zLimSpawnFar = -2.5f;

    public float effectsOnScreen = 0;
    float[] spawnIntervals = { 8, 10, 12 };
    float timeToNextSpawn;
    float spawnTimer = 0;

    float[] coinNumbers = { 4, 5, 6 };
    float coinsToSpawn;
    float[] coinSpawnIntervals = { 12, 15, 18 };
    float timeToNextCoinSpawn;
    float coinSpawnTimer = 0;

    public Transform ball;
    public Transform player;
    //public Transform opponent;
    LogicVolleyball scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.Find("Logic Volleyball").GetComponent<LogicVolleyball>();
        SelectSpawnInterval();
        SelectCoinSpawns();
        timeToNextCoinSpawn = 1; // spawn coins almost at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer < timeToNextSpawn && effectsOnScreen < 2 && ball.GetComponent<Volleyball>().inPlay)
        {
            spawnTimer += Time.deltaTime;
        }
        else if (spawnTimer >= timeToNextSpawn)
        {
            SpawnEffect();
            SelectSpawnInterval();
            spawnTimer = 0;
        }

        if (coinSpawnTimer < timeToNextCoinSpawn && ball.GetComponent<Volleyball>().inPlay)
        {
            coinSpawnTimer += Time.deltaTime;
        }
        else if (coinSpawnTimer >= timeToNextCoinSpawn)
        {
            SpawnCoins(6);
            SelectCoinSpawns();
            coinSpawnTimer = 0;
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

    void SpawnCoins(int numCoins)
    {
        Vector3[] coinPositions = new Vector3[numCoins];
        // select coins randomly and spawn at random positions
        for (int i = 0; i < numCoins; i++)
        {
            int randNum = Random.Range(0, coins.Length);

            // prevent coins from spawning within 1 unit of each other
            Vector3 randSpawnPos;
            while (true)
            {
                randSpawnPos = new Vector3(Random.Range(-xLimSpawn, xLimSpawn), ySpawn, Random.Range(zLimSpawnNear, zLimSpawnFar));

                float xMinDist = 100;
                float zMinDist = 100;
                for (int j = 0; j < i; j++)
                {
                    if (Mathf.Abs(randSpawnPos.x - coinPositions[j].x) < xMinDist)
                        xMinDist = Mathf.Abs(randSpawnPos.x - coinPositions[j].x);
                    if (Mathf.Abs(randSpawnPos.z - coinPositions[j].z) < xMinDist)
                        zMinDist = Mathf.Abs(randSpawnPos.z - coinPositions[j].z);
                }

                if (Mathf.Min(xMinDist, zMinDist) > 1)
                    break;
            }

            coinPositions[i] = randSpawnPos;
            // use same script for all coin types; assign point value here
            GameObject newCoin = Instantiate(coins[randNum], randSpawnPos, Quaternion.Euler(new Vector3(-90, 0, 0)));
            newCoin.GetComponent<CoinVolleyball>().pointValue = randNum + 1; // copper = 1, silver = 2, gold = 3
        }
    }

    void SelectSpawnInterval()
    {
        int randNum = Random.Range(0, spawnIntervals.Length);
        timeToNextSpawn = spawnIntervals[randNum];
    }
    void SelectCoinSpawns()
    {
        int randNum1 = Random.Range(0, coinNumbers.Length);
        coinsToSpawn = coinNumbers[randNum1];

        int randNum2 = Random.Range(0, coinSpawnIntervals.Length);
        timeToNextCoinSpawn = coinSpawnIntervals[randNum2];
    }

    public void EnableEffect(string effect)
    {
        if (effect == "Super Speed")
            player.GetComponent<PlayerVolleyball>().EnableSuperSpeed();
        else if (effect == "Double Points")
            scoreManager.EnableDoublePoints();
    }
}
