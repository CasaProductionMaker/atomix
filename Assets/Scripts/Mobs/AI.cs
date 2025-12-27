using Unity.Netcode;
using UnityEngine;

public class AI : Mob
{
    public float explosionDamage = 80f;
    public float explosionRadius = 3f;
    public GameObject explosionEffect;

    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        if (!IsOwner) return;
        DetectTarget();
        MoveTowardsTarget();
        LookAtTarget();
        CheckCollisions();
        TickVelocity();
        DieIfDead();
    }

    void LookAtTarget()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GFXRotation.Value = angle;
    }

    public override void OnDamaged()
    {
        spawnExplosionServerRpc(explosionRadius);

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach(Collider2D collider in hit)
        {
            if(collider.TryGetComponent(out Player player))
            {
                player.TakeDamageOwnerRpc(explosionDamage);
            }
        }
        health.Value = 0;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void spawnExplosionServerRpc(float explosionRadius)
    {
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explosion.GetComponent<NetworkObject>().Spawn(true);
        explosion.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
    }
}
