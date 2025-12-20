using UnityEngine;

public class AISummon : Summon
{
    public float explosionDamage = 80f;
    public float explosionRadius = 3f;
    public GameObject explosionEffect;

    void Update()
    {
        DetectTarget();
        MoveTowardsTarget();
        LookAtTarget();
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
        DieIfSpawnerDead();
    }

    void LookAtTarget()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GFX.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public override void OnDamaged()
    {
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach(Collider2D collider in hit)
        {
            if(collider.TryGetComponent(out Mob mob))
            {
                mob.TakeDamageServerRpc(explosionDamage);
            }
        }
        health = 0;
    }

    void DieIfSpawnerDead()
    {
        if (!parentElectron)
        {
            Destroy(gameObject);
        }
    }
}
