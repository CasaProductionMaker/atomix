using UnityEngine;
using Unity.Netcode;

public class Mine : Electron
{
    public float explosionRadius = 5f;
    public float explosionDamage = 100f;
    public GameObject explosionEffects;

    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        StayInBounds();
        CheckCollisions();
        TurnAnimation();
    }

    public bool isActive()
    {
        return Time.time - timeDied >= reload + secondaryReload;
    }

    new void CheckCollisions()
    {
        if (isDead) return;
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, size);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob _))
            {
                spawnExplosionServerRpc(explosionRadius);

                Collider2D[] explodeHit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
                foreach(Collider2D explodeCollider in explodeHit)
                {
                    if(explodeCollider.TryGetComponent(out Player player))
                    {
                        player.TakeDamageOwnerRpc(explosionDamage);
                    }
                    if(explodeCollider.TryGetComponent(out Mob mob))
                    {
                        mob.TakeDamageServerRpc(explosionDamage);
                    }
                }
                health = 0;
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void spawnExplosionServerRpc(float explosionRadius)
    {
        GameObject explosion = Instantiate(explosionEffects, transform.position, Quaternion.identity);
        explosion.GetComponent<NetworkObject>().Spawn(true);
        explosion.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
    }
}
