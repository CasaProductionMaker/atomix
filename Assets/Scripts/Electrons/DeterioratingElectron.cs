using UnityEngine;
using Unity.Netcode;

public class DeterioratingElectron : Electron
{
    public float deterioration = 3f;
    public float duration = 5f;
    public GameObject poisonEffectPrefab;
    void Update()
    {
        UpdateVisuals();
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
                SpawnPosionEffectServerRpc(mob.transform.position, mob.GetComponent<NetworkObject>().NetworkObjectId, deterioration, duration);

                Neutralizer neutralizer = player.GetActiveNeutralizer();
                if (neutralizer != null)
                {
                    mob.transform.position = neutralizer.transform.position;
                }
            }

            if (collider.gameObject.TryGetComponent(out AntiElectron antiElectron))
            {
                if (antiElectron.isDead) continue;
                antiElectron.TakeDamageOwnerRpc(GetDamage());
                health -= antiElectron.GetDamage();

                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                float totalDistance = size + (collider as CircleCollider2D).radius;
                float multiplier = totalDistance - (collider.transform.position - transform.position).magnitude;

                transform.position += (Vector3)(-hitAngle * multiplier);
                antiElectron.ApplyVelocityOwnerRpc(hitAngle * 0.02f);
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SpawnPosionEffectServerRpc(Vector3 position, ulong mobID, float deterioration, float duration)
    {
        GameObject spawnedPoisonEffect = Instantiate(poisonEffectPrefab, position, Quaternion.identity);
        spawnedPoisonEffect.GetComponent<PoisonEffect>().Initialize(deterioration, duration, NetworkManager.SpawnManager.SpawnedObjects[mobID].GetComponent<Mob>());
        spawnedPoisonEffect.GetComponent<NetworkObject>().Spawn(true);
    }
}
