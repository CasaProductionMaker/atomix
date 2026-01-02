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
        spawnRadiationEffectClientRpc();
        
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

    [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Everyone)]
    public void spawnRadiationEffectClientRpc()
    {
        Instantiate(radiationEffect, transform);
    }
}
