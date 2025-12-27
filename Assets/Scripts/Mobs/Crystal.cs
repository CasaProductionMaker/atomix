using UnityEngine;
using Unity.Netcode;

public class Crystal : Mob
{
    public float radiation = 5;
    public float radius = 5;
    public GameObject radiationEffect;
    float lastHit = 0;
    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        DoRadiation();
        DieIfDead();
    }

    void DoRadiation()
    {
        if (Time.time - lastHit < 1) return;
        spawnRadiationEffectServerRpc(GetComponent<NetworkObject>().NetworkObjectId);
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Player player))
            {
                player.TakeDamageOwnerRpc(radiation);
            }
        }
        lastHit = Time.time;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void spawnRadiationEffectServerRpc(ulong parentID)
    {
        NetworkObject electron = NetworkManager.SpawnManager.SpawnedObjects[parentID];

        GameObject radiationInstance = Instantiate(radiationEffect);
        NetworkObject radiationNO = radiationInstance.GetComponent<NetworkObject>();

        radiationNO.Spawn(true);

        radiationNO.TrySetParent(electron, true);
    }
}
