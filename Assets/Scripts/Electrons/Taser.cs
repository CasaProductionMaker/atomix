using UnityEngine;
using Unity.Netcode;

public class Taser : Electron
{
    public float duration = 5f;
    public GameObject stunEffectPrefab;
    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        TurnAnimation();
    }

    new void CheckCollisions()
    {
        if (isDead) return;
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, size);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                mob.TakeDamageServerRpc(damage);
                health -= mob.bodyDamage;
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                float totalDistance = size + (collider as CircleCollider2D).radius;
                float multiplier = totalDistance - (collider.transform.position - transform.position).magnitude;
                transform.position += (Vector3)(-hitAngle * multiplier);
                mob.MoveVectorServerRpc(hitAngle * 0.02f);
                SpawnStunEffectServerRpc(mob.transform.position, mob.GetComponent<NetworkObject>().NetworkObjectId, duration);

                Neutralizer neutralizer = player.GetActiveNeutralizer();
                if (neutralizer != null)
                {
                    mob.transform.position = neutralizer.transform.position;
                }
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SpawnStunEffectServerRpc(Vector3 position, ulong mobID, float duration)
    {
        GameObject spawnedStunEffect = Instantiate(stunEffectPrefab, position, Quaternion.identity);
        spawnedStunEffect.GetComponent<StunEffect>().Initialize(duration, NetworkManager.SpawnManager.SpawnedObjects[mobID].GetComponent<Mob>());
        spawnedStunEffect.GetComponent<NetworkObject>().Spawn(true);
    }
}
