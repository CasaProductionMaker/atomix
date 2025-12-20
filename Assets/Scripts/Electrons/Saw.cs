using UnityEngine;

public class Saw : Electron
{
    Vector2 velocity;
    public float speed = 10f;
    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        StayInBounds();
        if (isDetached) MoveSaw();
        CheckCollisions();
        TurnAnimation();
    }

    new void TurnAnimation()
    {
        transform.Rotate(0, 0, 270f * Time.deltaTime);
    }
    public void Fire(Vector2 direction)
    {
        Vector2 normalizedDirection = direction.normalized;
        velocity = normalizedDirection * speed;
    }

    void MoveSaw()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
        if (isOutOfBounds())
        {
            Vector2 hitAngle = Vector2.zero;
            Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize + new Vector2(0.5f, 0.5f);
            if (transform.position.x + size > bounds.x)
            {
                hitAngle = new Vector2(-1, 0);
            }
            if (transform.position.y + size > bounds.y)
            {
                hitAngle = new Vector2(0, -1);
            }
            if (transform.position.x - size < -bounds.x)
            {
                hitAngle = new Vector2(1, 0);
            }
            if (transform.position.y - size < -bounds.y)
            {
                hitAngle = new Vector2(0, 1);
            }
            velocity = Vector2.Reflect(velocity, hitAngle);
        }
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
                if (isDetached)
                {
                    velocity = Vector2.Reflect(velocity, hitAngle);
                    return;
                }
                float totalDistance = size + (collider as CircleCollider2D).radius;
                float multiplier = totalDistance - (collider.transform.position - transform.position).magnitude;
                transform.position += (Vector3)(-hitAngle * multiplier);
                mob.MoveVectorServerRpc(hitAngle * 0.02f);
            }
        }
    }
}