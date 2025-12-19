using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;

public class MobSpawner : NetworkBehaviour
{
    public Vector2 mapSize = new Vector2(20f, 20f);
    public Transform[] borders;
    public List<Transform> playerTransforms = new List<Transform>();

    public SpawnableMob[] spawnableMobs;
    private float lastSpawnTime = 0f;

    public int waveNumber = 1;
    public int mobsSpawnedInWave = 0;
    public int mobsLeftInWave = 0;
    public bool isSpawningMobs = false;

    public TextMeshProUGUI waveText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Position
        borders[0].transform.position = new Vector3(mapSize.x + 0.75f, 0, 0);
        borders[1].transform.position = new Vector3(0, -mapSize.y - 0.75f, 0);
        borders[2].transform.position = new Vector3(-mapSize.x - 0.75f, 0, 0);
        borders[3].transform.position = new Vector3(0, mapSize.y + 0.75f, 0);

        //Scale
        borders[0].transform.localScale = new Vector3(0.5f, 2 * mapSize.y + 2, 1);
        borders[1].transform.localScale = new Vector3(2 * mapSize.x + 2, 0.5f, 1);
        borders[2].transform.localScale = new Vector3(0.5f, 2 * mapSize.y + 2, 1);
        borders[3].transform.localScale = new Vector3(2 * mapSize.x + 2, 0.5f, 1);
    }

    public void startGame()
    {
        isSpawningMobs = true;
        lastSpawnTime = Time.time;
        mobsLeftInWave = waveNumber * 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawningMobs) { return; }
        UpdatePlayerTransforms();
        if (mobsLeftInWave <= 1 && mobsSpawnedInWave >= waveNumber * 2)
        {
            Debug.Log("Next wave!");
            waveNumber++;
            mobsSpawnedInWave = 0;
            mobsLeftInWave += waveNumber * 2;
            waveText.text = "Wave " + waveNumber;
        }

        if (Time.time - lastSpawnTime >= GetSpawnInterval() && mobsSpawnedInWave < waveNumber * 2)
        {
            SpawnMob();
            mobsSpawnedInWave++;
            lastSpawnTime = Time.time;
        }
    }

    public void UpdatePlayerTransforms()
    {
        playerTransforms.Clear();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach(GameObject player in players)
        {
            playerTransforms.Add(player.transform);
        }
    }

    bool IsPlayerNearPosition(Vector2 position)
    {
        foreach(Transform player in playerTransforms) {
            if(Vector2.Distance(position, (Vector2)player.position) < mapSize.x / 2f)
            {
                return true;
            }
        }
        return false;
    }

    public void SpawnMob()
    {
        float x = Random.Range(-mapSize.x, mapSize.x);
        float y = Random.Range(-mapSize.y, mapSize.y);

        while (IsPlayerNearPosition(new Vector2(x, y)))
        {
            x = Random.Range(-mapSize.x, mapSize.x);
            y = Random.Range(-mapSize.y, mapSize.y);
        }

        Vector2 spawnPosition = new Vector2(x, y);

        List<SpawnableMob> waveSpawnableMobs = new List<SpawnableMob>();

        foreach (SpawnableMob mob in spawnableMobs)
        {
            if (mob.firstWave <= waveNumber)
            {
                waveSpawnableMobs.Add(mob);
            }
        }

        int prefabIndex = Random.Range(0, waveSpawnableMobs.Count);
        GameObject mobPrefab = waveSpawnableMobs[prefabIndex].mobPrefab;

        GameObject newMob = Instantiate(mobPrefab, spawnPosition, Quaternion.identity);
        newMob.GetComponent<NetworkObject>().Spawn();
    }

    float GetSpawnInterval()
    {
        return Mathf.Max(0.5f, 2f - (waveNumber * 0.1f));
    }
}

[System.Serializable]
public struct SpawnableMob
{
    public GameObject mobPrefab;
    public int firstWave;
}