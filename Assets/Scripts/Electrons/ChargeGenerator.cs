using UnityEngine;

public class ChargeGenerator : Electron
{
    public GameObject electricStormPrefab;
    GameObject electricStormInstance;
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
        if (!hasSpawnedStorm() && (Time.time - timeDied) >= reload + secondaryReload)
        {
            electricStormInstance = Instantiate(electricStormPrefab, transform.position, Quaternion.identity);
            electricStormInstance.GetComponent<ElectricStormSummon>().parentElectron = this;
        }
    }

    public bool hasSpawnedStorm()
    {
        return electricStormInstance != null;
    }

    public new void DieIfDead()
    {
        if (isPlayerDead) health = 0;
        if(health <= 0 && !isDead)
        {
            timeDied = Time.time;
            isDead = true;
            Destroy(electricStormInstance);
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
        if (electricStormInstance != null)
        {
            electricStormInstance.transform.position += (Vector3)moveVector * Time.deltaTime * 2f;
        }
    }
}
