using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MobSpawner : MonoBehaviour
{
    public Vector2 mapSize = new Vector2(20f, 20f);
    public Transform playerTransform;

    public SpawnableMob[] spawnableMobs;
    private float lastSpawnTime = 0f;

    public int waveNumber = 1;
    public int mobsSpawnedInWave = 0;
    public int mobsLeftInWave = 0;

    public TextMeshProUGUI waveText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastSpawnTime = Time.time;
        mobsLeftInWave = waveNumber * 2;
    }

    // Update is called once per frame
    void Update()
    {
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

    public void SpawnMob()
    {
        float x = Random.Range(-mapSize.x, mapSize.x);
        float y = Random.Range(-mapSize.y, mapSize.y);

        while (Vector2.Distance(new Vector2(x, y), new Vector2(playerTransform.position.x, playerTransform.position.y)) < 5f)
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

        Instantiate(mobPrefab, spawnPosition, Quaternion.identity);
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