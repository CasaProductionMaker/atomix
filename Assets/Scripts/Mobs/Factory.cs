using Unity.Netcode;
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
                spawnAIServerRpc(spawnPosition.position);
                FindFirstObjectByType<MobSpawner>().mobsLeftInWave++;
            }
            lastSpawnTime = Time.time;
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void spawnAIServerRpc(Vector3 position)
    {
        GameObject AIInstance = Instantiate(AIPrefab, position, Quaternion.identity);
        AIInstance.GetComponent<NetworkObject>().Spawn(true);
    }
}
