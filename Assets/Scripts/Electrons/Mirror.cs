using UnityEngine;

public class Mirror : Electron
{
    public float reflection = 0.5f;
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
                mob.TakeDamageServerRpc(mob.bodyDamage * reflection);
                health -= mob.bodyDamage;
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                float totalDistance = size + (collider as CircleCollider2D).radius;
                float multiplier = totalDistance - (collider.transform.position - transform.position).magnitude;
                transform.position += (Vector3)(-hitAngle * multiplier);
                mob.MoveVectorServerRpc(hitAngle * 0.02f);
            }

            if (collider.gameObject.TryGetComponent(out AntiElectron antiElectron))
            {
                if (antiElectron.isDead) continue;
                antiElectron.TakeDamageOwnerRpc(antiElectron.GetDamage() * reflection);
                health -= antiElectron.GetDamage();

                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                float totalDistance = size + (collider as CircleCollider2D).radius;
                float multiplier = totalDistance - (collider.transform.position - transform.position).magnitude;

                transform.position += (Vector3)(-hitAngle * multiplier);
                antiElectron.ApplyVelocityOwnerRpc(hitAngle * 0.02f);
            }
        }
    }
}
