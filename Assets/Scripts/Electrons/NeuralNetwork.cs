using UnityEngine;

public class NeuralNetwork : Electron
{
    public GameObject AIPrefab;
    GameObject AIInstance;
    void Update()
    {
        DieIfDead();
        if (isDead) return;
        StayInBounds();
        SpawnElectricStorm();
        CheckCollisions();
        TurnAnimation();
    }

    void SpawnElectricStorm()
    {
        if (!hasSpawnedAI() && (Time.time - timeDied) >= reload + secondaryReload)
        {
            AIInstance = Instantiate(AIPrefab, transform.position, Quaternion.identity);
            AIInstance.GetComponent<AISummon>().parentElectron = this;
        }
    }

    public bool hasSpawnedAI()
    {
        return AIInstance != null;
    }

    public new void DieIfDead()
    {
        if (isPlayerDead) health = 0;
        if(health <= 0 && !isDead)
        {
            timeDied = Time.time;
            isDead = true;
            Destroy(AIInstance);
            Debug.Log("???");
        }
        if (isDead)
        {
            if (Time.time - timeDied >= reload)
            {
                health = maxHealth;
                isDead = false;
            }
        }
    }

    public void MoveSpawned(Vector2 moveVector)
    {
        if (AIInstance != null)
        {
            AIInstance.transform.position += (Vector3)moveVector * Time.deltaTime * 2f;
        }
    }
}
