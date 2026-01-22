using UnityEngine;
using Unity.Netcode;

public class Isotope : Electron
{
    public float radiation = 5;
    public float radius = 3;
    public GameObject radiationEffect;
    float lastHit = 0;
    void Update()
    {
        UpdateVisuals();
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        DoRadiation();
        TurnAnimation();
    }

    void DoRadiation()
    {
        if (isDead) return;
        if (Time.time - lastHit < 1) return;
        spawnRadiationEffectClientRpc();
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                mob.TakeDamageServerRpc(radiation);
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
