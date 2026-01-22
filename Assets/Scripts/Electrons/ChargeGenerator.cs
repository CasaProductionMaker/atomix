using Unity.Netcode;
using UnityEngine;

public class ChargeGenerator : Electron
{
    public GameObject electricStormPrefab;
    GameObject electricStormInstance;
    void Update()
    {
        UpdateVisuals();
        if (!IsOwner) return;
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
            SpawnStormServerRpc(NetworkManager.LocalClientId);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SpawnStormServerRpc(ulong clientID)
    {
        GameObject spawnedStorm = Instantiate(electricStormPrefab, transform.position, Quaternion.identity);
        spawnedStorm.GetComponent<NetworkObject>().Spawn(true);
        spawnedStorm.GetComponent<NetworkObject>().ChangeOwnership(clientID);

        ReturnWithSpawnedStormClientRpc(spawnedStorm.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ReturnWithSpawnedStormClientRpc(ulong networkObjectId)
    {
        if (!IsOwner) return;
        GameObject stormObject = NetworkManager.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
        Debug.Log(stormObject);
        electricStormInstance = stormObject;
        electricStormInstance.GetComponent<ElectricStormSummon>().parentElectron = this;
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
            if (electricStormInstance) electricStormInstance.GetComponent<ElectricStormSummon>().DestroySelfServerRpc();
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
