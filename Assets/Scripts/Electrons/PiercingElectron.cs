using UnityEngine;
using UnityEngine.Rendering;

public class PiercingElectron : Electron
{
    public float damageMultiplier = 10;
    void Update()
    {
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
    }

    new void CheckCollisions()
    {
        if (isDead) return;
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, size);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                float actualDamageMultiplier = 1f;
                if (mob.health.Value / mob.maxHealth >= 0.8f) actualDamageMultiplier = damageMultiplier;
                mob.TakeDamage(GetDamage() * actualDamageMultiplier, gameObject);
                health -= mob.bodyDamage;
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                float totalDistance = size + (collider as CircleCollider2D).radius;
                float multiplier = totalDistance - (collider.transform.position - transform.position).magnitude;
                transform.position += (Vector3)(-hitAngle * multiplier);
                mob.MoveVector(hitAngle * 0.02f);
            }
        }
    }
}
