using UnityEngine;

public class Factory : Mob
{
    public GameObject AIPrefab;
    public Transform spawnPosition;
    public float spawnRate = 10f;
    public int spawnAmount = 3;
    float lastSpawnTime = 0f;

    new void Start()
    {
        lastSpawnTime = Time.time;
        base.Start();
    }
    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        SpawnMobs();
        DieIfDead();
    }

    void SpawnMobs()
    {
        if (Time.time - lastSpawnTime >= spawnRate)
        {
            for (int i = 0; i < spawnAmount; i++)
            {
                Instantiate(AIPrefab, spawnPosition.position, Quaternion.identity);
                FindFirstObjectByType<MobSpawner>().mobsLeftInWave++;
            }
            lastSpawnTime = Time.time;
        }
    }
}
