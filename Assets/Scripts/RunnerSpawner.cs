using System.Collections.Generic;
using UnityEngine;

public class RunnerSpawner : MonoBehaviour
{
    public Obstacle obstaclePrefab;
    public Transform parent;

    [Header("Spawning")]
    public float spawnInterval = 2f;
    public float minSpawnInterval = 0.8f;
    public float spawnDistance = 1200f;
    public float laneDistance = 6f;

    [Header("Movement")]
    public float obstacleSpeed = 350f;
    public float playerZ = 500f;

    [Header("Difficulty Ramp")]
    public float speedGainPerSecond = 2f;
    public float intervalLossPerSecond = 0.01f;

    private float spawnTimer;
    private List<Obstacle> obstacles = new List<Obstacle>();

    private void Update()
    {
        if (obstaclePrefab == null || parent == null)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnObstacle();
            spawnTimer = 0f;
        }

        MoveObstacles();

        obstacleSpeed += speedGainPerSecond * Time.deltaTime;
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval - intervalLossPerSecond * Time.deltaTime);
    }

    private void SpawnObstacle()
    {
        int lane = Random.Range(0, 3);
        float x = (lane - 1) * laneDistance;

        Obstacle obstacle = Instantiate(obstaclePrefab, parent);
        obstacle.item.itemPosition = new Vector3(x, -8f, spawnDistance);
        obstacle.item.ApplyPerspective();

        obstacles.Add(obstacle);
    }

    private void MoveObstacles()
    {
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            Obstacle obstacle = obstacles[i];

            if (obstacle == null)
            {
                obstacles.RemoveAt(i);
                continue;
            }

            obstacle.item.itemPosition.z -= obstacleSpeed * Time.deltaTime;

            // destroy once well past the player
            if (obstacle.item.itemPosition.z < playerZ - 60f)
            {
                Destroy(obstacle.gameObject);
                obstacles.RemoveAt(i);
            }
        }
    }
}