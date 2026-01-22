using Unity.Netcode;
using UnityEngine;

public class Bolt : Electron
{
    public float lightningDamage;
    public float zapRange;
    public GameObject lightningEffectPrefab;
    void Update()
    {
        UpdateVisuals();
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        ZapIfPossible();
        TurnAnimation();
    }

    void ZapIfPossible()
    {
        if (isDead) return;
        if (GetTimeSinceDied() < reload + secondaryReload) return;
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, zapRange);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                // Summon lightning effect
                mob.TakeDamageServerRpc(lightningDamage);
                spawnLightningEffectServerRpc(transform.position, mob.transform.position);
                health = 0;
                return;
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void spawnLightningEffectServerRpc(Vector3 initPos, Vector3 endPos)
    {
        spawnLightningEffectClientRpc(initPos, endPos);
    }

    [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Everyone)]
    public void spawnLightningEffectClientRpc(Vector3 initPos, Vector3 endPos)
    {
        GameObject lightningEffect = Instantiate(lightningEffectPrefab, endPos, Quaternion.identity);
        lightningEffect.GetComponent<LightningEffect>().Initialize(initPos, endPos);
    }
}
