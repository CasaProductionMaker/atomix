using UnityEngine;

public class Crystal : Mob
{
    public float radiation = 5;
    public float radius = 5;
    public GameObject radiationEffect;
    float lastHit = 0;
    void Update()
    {
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DoRadiation();
        DieIfDead();
    }

    void DoRadiation()
    {
        if (Time.time - lastHit < 1) return;
        Instantiate(radiationEffect, transform);
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Player player))
            {
                player.TakeDamage(radiation);
            }
        }
        lastHit = Time.time;
    }
}
