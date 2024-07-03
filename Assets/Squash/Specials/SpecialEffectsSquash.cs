using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEffectsSquash : MonoBehaviour
{
    public GameObject[] effects;
    public GameObject[] coins;
    public GameObject[] animals;
    float xLimSpawn = 7f;
    float ySpawn = 51.8f;
    float zLimSpawnNear = -9f;
    float zLimSpawnFar = 8f;

    public float effectsOnScreen = 0;
    float[] spawnIntervals = { 6, 8, 10 };
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
    LogicSquash scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.Find("Logic Squash").GetComponent<LogicSquash>();
        SelectSpawnInterval();
        SelectCoinSpawns();
        timeToNextCoinSpawn = 1; // spawn coins almost at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTimer < timeToNextSpawn && effectsOnScreen < 2 && ball.GetComponent<SquashBall>().inPlay)
        {
            spawnTimer += Time.deltaTime;
        }
        else if (spawnTimer >= timeToNextSpawn)
        {
            SpawnEffect();
            SelectSpawnInterval();
            spawnTimer = 0;
        }

        if (coinSpawnTimer < timeToNextCoinSpawn && ball.GetComponent<SquashBall>().inPlay)
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
            newCoin.GetComponent<Coin>().pointValue = randNum + 1; // copper = 1, silver = 2, gold = 3
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
            player.GetComponent<PlayerSquash>().EnableSuperSpeed();
        else if (effect == "Double Points")
            scoreManager.EnableDoublePoints();
        else if (effect == "Extra Time")
            scoreManager.AddExtraTime();
    }

    public void SpawnAnimal(Vector3 initPos)
    {
        int randNum = Random.Range(0, animals.Length);
        float randAngle = Random.Range(78, 88);
        if (initPos.x < 0)
            randAngle *= -1;

        Instantiate(animals[randNum], initPos, Quaternion.Euler(new Vector3(0, randAngle, 0)));
    }
}
