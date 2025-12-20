using UnityEngine;

public class Magnet : Electron
{
    public float pull = 0.3f;
    public float radius = 3;
    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        DoMagnetism();
        TurnAnimation();
    }

    void DoMagnetism()
    {
        if (isDead) return;
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                Vector2 pullDirection = transform.position - mob.transform.position;
                float pullAmount = pull / pullDirection.magnitude;
                pullDirection = pullDirection.normalized * pullAmount;
                mob.ApplyVelocityServerRpc(pullDirection);
            }
        }
    }
}
