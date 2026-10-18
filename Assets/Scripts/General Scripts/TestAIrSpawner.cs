using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Splines;

public class TestAIrSpawner : MonoBehaviour
{
    [SerializeField] float minSpawnRadius = 200f;
    [SerializeField] float maxSpawnRadius = 300f;
    [SerializeField] float minSpawnHeight = 30f;
    [SerializeField] float maxSpawnHeight = 40f;
    [SerializeField] int maxEnemyCount = 5;
    [SerializeField] Transform targetShip;
    [SerializeField] AircraftBase[] aircraftPrefabs;
    [SerializeField] GameManager gameManager;
    float spawnTimer = 0f;
    [SerializeField] float  spawnTime = 30f;
    bool startSpawn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawn)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnTime)
            {
                Spawn(1);
                spawnTimer = 0f;
                if (gameManager.GetEnemyCount() == maxEnemyCount) startSpawn = false;
            }
        }
        if (gameManager.GetEnemyCount() == 0)
        {
            Spawn();
        }
        else if (gameManager.GetEnemyCount() <= 1f/3f * maxEnemyCount)
        {
            startSpawn = true;
        }
    }
    void Spawn(int spawnCount = -1)
    {
        if (spawnCount == -1) spawnCount = maxEnemyCount;
        HashSet<Vector3> spawnPoints = new();
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 onCircle = Random.onUnitCircle;
            float radius = Random.Range(minSpawnRadius, maxSpawnRadius);
            float height = Random.Range(minSpawnHeight, maxSpawnHeight);
            Vector3 spawnPoint = new(onCircle.x * radius, height, onCircle.y * radius);
            while (spawnPoints.Contains(spawnPoint))
            {
                onCircle = Random.onUnitCircle;
                radius = Random.Range(minSpawnRadius, maxSpawnRadius);
                height = Random.Range(minSpawnHeight, maxSpawnHeight);
                spawnPoint.Set(onCircle.x * radius, height, onCircle.y * radius);
            }
            spawnPoints.Add(spawnPoint);
            Vector3 spawnDirection = targetShip.position - spawnPoint;
            AircraftBase aircraft = Instantiate(
                aircraftPrefabs[Random.Range(0, aircraftPrefabs.Length)],
                spawnPoint,
                Quaternion.LookRotation(spawnDirection)
            );
            gameManager.RegistryAircraft(aircraft);
        }
    }
}
