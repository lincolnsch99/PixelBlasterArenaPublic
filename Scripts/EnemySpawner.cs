using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static float INIT_SHREDDER_RATE = 1f;
    public static float INIT_PURSUER_RATE = 0f;
    public static float INIT_BUSTER_RATE = 0f;
    public static int INIT_NUM_ENEMIES = 5;
    public static float INIT_SPAWN_FREQ = 3f;

    private static float MAX_PURSUER_RATE = 0.4f;
    private static float MAX_BUSTER_RATE = 0.5f;
    private static float MIN_SPAWN_FREQ = 0.5f;
    private static int SPAWN_AREA_HEIGHT = 25;
    private static int SPAWN_AREA_WIDTH = 5;
    private static int SPAWN_AREA_START = 25;

    [SerializeField]
    private int waveResetFreq;
    [SerializeField]
    private GameObject ShredderPrefab;
    [SerializeField]
    private GameObject PursuerPrefab;
    [SerializeField]
    private GameObject BusterPrefab;

    private Dictionary<EnemyType, float> spawnRates;
    private Stack<EnemyType> enemiesToSpawn;
    private int roundsPassed, enemyCount;
    private float spawnFrequency, roundMultiplier, curveStart, curveEnd, waveDropMagnitude;

    private float timer;

    private void Awake()
    {
        spawnRates = new Dictionary<EnemyType, float>();
        InitializeSpawnRates();
        roundsPassed = 0;
        spawnFrequency = INIT_SPAWN_FREQ;
        enemyCount = INIT_NUM_ENEMIES;
        roundMultiplier = 1.1f;
        waveDropMagnitude = 0.6f;
        curveStart = enemyCount;
        enemiesToSpawn = GetSpawns();

        timer = 0;
    }

    private void InitializeSpawnRates()
    {
        if (spawnRates.Count > 0)
            spawnRates.Clear();
        spawnRates.Add(EnemyType.SHREDDER, INIT_SHREDDER_RATE);
        spawnRates.Add(EnemyType.PURSUER, INIT_PURSUER_RATE);
        spawnRates.Add(EnemyType.BUSTER, INIT_BUSTER_RATE);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= spawnFrequency)
        {
            if(enemiesToSpawn.Count > 0)
            {
                SpawnOneEnemy(enemiesToSpawn.Pop());
            }
            else
            {
                IncrementRound();
                enemiesToSpawn = GetSpawns();
            }
            timer = 0;
        }
    }

    private void IncrementRound()
    {
        roundsPassed++;
        if (roundsPassed % waveResetFreq == 0)
        {
            curveEnd = enemyCount;
            enemyCount -= Mathf.CeilToInt(((curveEnd - curveStart) * waveDropMagnitude));
            curveStart = enemyCount;
        }
        else
        {
            enemyCount += Mathf.CeilToInt(roundMultiplier * 3f);

            if (spawnRates[EnemyType.PURSUER] < MAX_PURSUER_RATE)
                spawnRates[EnemyType.PURSUER] += 0.05f;
            if (spawnRates[EnemyType.BUSTER] < MAX_BUSTER_RATE)
                spawnRates[EnemyType.BUSTER] += 0.025f;
            spawnRates[EnemyType.SHREDDER] = 1f - (spawnRates[EnemyType.PURSUER] + spawnRates[EnemyType.BUSTER]);

            if (spawnFrequency > MIN_SPAWN_FREQ)
                spawnFrequency -= 0.2f;
        }
    }

    private Stack<EnemyType> GetSpawns()
    {
        Stack<EnemyType> enemies = new Stack<EnemyType>();

        for(int i = 0; i < enemyCount; i++)
        {
            float rand = Random.value;
            if (rand <= spawnRates[EnemyType.SHREDDER])
                enemies.Push(EnemyType.SHREDDER);
            else if (rand <= spawnRates[EnemyType.SHREDDER] + spawnRates[EnemyType.PURSUER])
                enemies.Push(EnemyType.PURSUER);
            else
                enemies.Push(EnemyType.BUSTER);
        }

        return enemies;
    }

    private void SpawnOneEnemy(EnemyType enemyType)
    {
        // Generate a random position outside of the boundaries.
        int rand = Random.Range(0, 4);
        float randPos1 = Random.Range(SPAWN_AREA_START, SPAWN_AREA_START + SPAWN_AREA_WIDTH);
        float randPos2 = Random.Range(-SPAWN_AREA_HEIGHT, SPAWN_AREA_HEIGHT + 1);

        float xPos, yPos;
        switch(rand)
        {
            case 0:
                xPos = randPos2;
                yPos = -randPos1;
                break;
            case 1:
                xPos = -randPos1;
                yPos = randPos2;
                break;
            case 2:
                xPos = randPos2;
                yPos = randPos1;
                break;
            case 3:
                xPos = randPos1;
                yPos = randPos2;
                break;
            default:
                xPos = randPos1;
                yPos = randPos2;
                break;
        }
        Vector3 spawnPos = new Vector3(xPos, yPos, 0);

        // Instantiate the correct enemy at the generated position.
        switch(enemyType)
        {
            case EnemyType.SHREDDER:
                GameObject.Instantiate(ShredderPrefab, spawnPos, new Quaternion());
                break;
            case EnemyType.PURSUER:
                GameObject.Instantiate(PursuerPrefab, spawnPos, new Quaternion());
                break;
            case EnemyType.BUSTER:
                GameObject.Instantiate(BusterPrefab, spawnPos, new Quaternion());
                break;
            default:
                GameObject.Instantiate(ShredderPrefab, spawnPos, new Quaternion());
                break;
        }
    }

}
