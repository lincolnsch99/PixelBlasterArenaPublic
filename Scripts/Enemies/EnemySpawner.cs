/// File Name: EnemySpawner.cs
/// File Author(s): Lincoln Schroeder
/// File Purpose: Handles all logic for enemy spawns.
/// 
/// Date Last Updated: November 26, 2019

using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static float INIT_SHREDDER_PCNT = 1f;
    public static float INIT_PURSUER_PCNT = 0f;
    public static float INIT_BUSTER_PCNT = 0f;
    public static int INIT_NUM_ENEMIES = 5;
    public static float INIT_SPAWN_FREQ = 2f;

    private static float MAX_PURSUER_PCNT = 0.4f;
    private static float MAX_BUSTER_PCNT = 0.5f;
    private static float MIN_SPAWN_FREQ = 0.5f;
    private static int SPAWN_AREA_SIZE = 100;

    private static int BRUTAL_RND_BONUS = 500;

    [Header("Spawn Utilities")]
    [SerializeField]
    private int waveResetFreq;
    [SerializeField]
    private GameObject ShredderPrefab;
    [SerializeField]
    private GameObject PursuerPrefab;
    [SerializeField]
    private GameObject BusterPrefab;
    [Header("Powerup Utilities")]
    [SerializeField]
    private GameObject shieldPowerupPrefab;
    [SerializeField]
    private GameObject overdrivePowerupPrefab;

    private Dictionary<EnemyType, float> spawnRates;
    private Stack<EnemyType> enemiesToSpawn;
    private int roundsPassed, enemyCount;
    private float spawnFrequency, roundMultiplier, curveStart, curveEnd, waveDropMagnitude;
    private PersistentControl persistentControl;
    private float spawnTimer;
    private bool powerups;

    /// <summary>
    /// Awake is called on the first frame of instatiation.
    /// </summary>
    private void Awake()
    {
        persistentControl = GameObject.FindWithTag("Persistent").GetComponent<PersistentControl>();
        roundMultiplier = persistentControl.SelectedStage.SpawnMultiplier;
        powerups = persistentControl.SelectedStage.CanPowerupsSpawn;
        spawnRates = new Dictionary<EnemyType, float>();
        InitializeSpawnRates();
        roundsPassed = 0;
        spawnFrequency = INIT_SPAWN_FREQ;
        enemyCount = INIT_NUM_ENEMIES;
        waveDropMagnitude = 0.6f;
        curveStart = enemyCount;
        enemiesToSpawn = new Stack<EnemyType>();

        spawnTimer = spawnFrequency;
    }

    /// <summary>
    /// Initializes the spawn rates to their starting values.
    /// </summary>
    private void InitializeSpawnRates()
    {
        if (spawnRates.Count > 0)
            spawnRates.Clear();
        spawnRates.Add(EnemyType.SHREDDER, INIT_SHREDDER_PCNT);
        spawnRates.Add(EnemyType.PURSUER, INIT_PURSUER_PCNT);
        spawnRates.Add(EnemyType.BUSTER, INIT_BUSTER_PCNT);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if(spawnTimer >= spawnFrequency)
        {
            if(enemiesToSpawn.Count > 0)
            {
                SpawnOneEnemy(enemiesToSpawn.Pop());
            }
            else
            {
                int activeEnemyCount = 0;
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                foreach(GameObject gObject in allObjects) 
                {
                    if (gObject.tag == "Enemy")
                        activeEnemyCount++;
                }
                if (activeEnemyCount == 0) /// This means all enemies have either been killed or removed from the scene.
                {
                    IncrementRound();
                    enemiesToSpawn = GetSpawns();
                }
            }
            spawnTimer = 0;
        }
    }

    /// <summary>
    /// Changes all necessary values for moving to the next round. Includes increasing the number
    /// of enemies, the rate at which they spawn, and the average difficulty of the enemies.
    /// </summary>
    private void IncrementRound()
    {
        roundsPassed++;
        if (roundsPassed % waveResetFreq == 0)
        {
            curveEnd = enemyCount;
            enemyCount -= Mathf.CeilToInt(((curveEnd - curveStart) * waveDropMagnitude));
            curveStart = enemyCount;
            spawnFrequency += 0.9f;
        }
        else
        {
            enemyCount += Mathf.CeilToInt(roundMultiplier * 3f);

            if (spawnRates[EnemyType.PURSUER] < MAX_PURSUER_PCNT)
                spawnRates[EnemyType.PURSUER] += 0.05f;
            if (spawnRates[EnemyType.BUSTER] < MAX_BUSTER_PCNT)
                spawnRates[EnemyType.BUSTER] += 0.025f;
            spawnRates[EnemyType.SHREDDER] = 1f - (spawnRates[EnemyType.PURSUER] + spawnRates[EnemyType.BUSTER]);

            if (spawnFrequency > MIN_SPAWN_FREQ)
                spawnFrequency -= 0.2f;
        }
        persistentControl.SetRound(roundsPassed);
    }

    /// <summary>
    /// Based on current spawn rates, enemies are decided randomly and put into a stack.
    /// </summary>
    /// <returns>Stack of EnemyTypes to be spawned.</returns>
    private Stack<EnemyType> GetSpawns()
    {
        Stack<EnemyType> enemies = new Stack<EnemyType>();

        int numBusters = Mathf.FloorToInt((float)enemyCount * spawnRates[EnemyType.BUSTER]); 
        int numPursuers = Mathf.FloorToInt((float)enemyCount * spawnRates[EnemyType.PURSUER]);
        int numShredders = enemyCount - (numPursuers + numBusters);

        while(enemies.Count < enemyCount)
        {
            int rand = Random.Range(0, 3);
            switch (rand)
            {
                case 0:
                    if(numShredders > 0)
                    {
                        enemies.Push(EnemyType.SHREDDER);
                        numShredders--;
                    }
                    break;
                case 1:
                    if (numPursuers > 0)
                    {
                        enemies.Push(EnemyType.PURSUER);
                        numPursuers--;
                    }
                    break;
                case 2:
                    if (numBusters > 0)
                    {
                        enemies.Push(EnemyType.BUSTER);
                        numBusters--;
                    }
                    break;
            }
        }
        return enemies;
    }

    /// <summary>
    /// Spawns the next enemy from the current round's stack. they spawn in random positions 
    /// around the edge of the boundary. Each spawn has a small chance to also spawn a powerup.
    /// </summary>
    /// <param name="enemyType">The type of enemy to be spawned.</param>
    private void SpawnOneEnemy(EnemyType enemyType)
    {
        Vector3 spawnPos = GetRandomSpawnPos();

        /// Instantiate the correct enemy at the generated position.
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

        if (powerups)
        {
            int powerupRand = Random.Range(0, 100);
            if (powerupRand < 2)
            {
                GameObject.Instantiate(shieldPowerupPrefab, GetRandomSpawnPos(), new Quaternion());
            }
            else if (powerupRand <= 4)
            {
                GameObject.Instantiate(overdrivePowerupPrefab, GetRandomSpawnPos(), new Quaternion());
            }
        }
    }

    /// <summary>
    /// Generates a random place to spawn.
    /// </summary>
    /// <returns>Vector3 representing a random spawn position.</returns>
    private Vector3 GetRandomSpawnPos()
    {
        /// Generate a random position outside of the boundaries.
        int rand = Random.Range(0, 4);
        float randPos1 = Random.Range(0, 8);
        float randPos2 = Random.Range(0, SPAWN_AREA_SIZE);

        float xPos, yPos;
        switch (rand)
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
                yPos = randPos1 + SPAWN_AREA_SIZE;
                break;
            case 3:
                xPos = randPos1;
                yPos = randPos2;
                break;
            default:
                xPos = randPos1 + SPAWN_AREA_SIZE;
                yPos = randPos2;
                break;
        }
        return new Vector3(xPos, yPos, 0);
    }

    /// <summary>
    /// Getter for the current number of enemies left to spawn.
    /// </summary>
    /// <returns>Size of the stack of enemies to be spawned.</returns>
    public int GetNumEnemiesRemaining()
    {
        return enemiesToSpawn.Count;
    }
}
