using UnityEngine;
using Unity.Netcode;

public class NeuralNetwork : Electron
{
    public GameObject AIPrefab;
    GameObject AIInstance;
    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        StayInBounds();
        SpawnAI();
        CheckCollisions();
        TurnAnimation();
    }

    void SpawnAI()
    {
        if (!hasSpawnedAI() && (Time.time - timeDied) >= reload + secondaryReload)
        {
            SpawnAIServerRpc(NetworkManager.LocalClientId);
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SpawnAIServerRpc(ulong clientID)
    {
        GameObject spawnedAI = Instantiate(AIPrefab, transform.position, Quaternion.identity);
        spawnedAI.GetComponent<NetworkObject>().Spawn(true);
        spawnedAI.GetComponent<NetworkObject>().ChangeOwnership(clientID);

        ReturnWithSpawnedAIClientRpc(spawnedAI.GetComponent<NetworkObject>().NetworkObjectId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ReturnWithSpawnedAIClientRpc(ulong networkObjectId)
    {
        if (!IsOwner) return;
        GameObject AIObject = NetworkManager.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
        AIInstance = AIObject;
        AIInstance.GetComponent<AISummon>().parentElectron = this;
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
            if (AIInstance != null) AIInstance.GetComponent<AISummon>().DestroySelfServerRpc();
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
