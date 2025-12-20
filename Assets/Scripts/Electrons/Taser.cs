using UnityEngine;

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
                Instantiate(stunEffectPrefab, mob.transform.position, Quaternion.identity).GetComponent<StunEffect>().Initialize(duration, mob);

                Neutralizer neutralizer = player.GetActiveNeutralizer();
                if (neutralizer != null)
                {
                    mob.transform.position = neutralizer.transform.position;
                }
            }
        }
    }
}
