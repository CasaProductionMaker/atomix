using UnityEngine;

public class Isotope : Electron
{
    public float radiation = 5;
    public float radius = 3;
    public GameObject radiationEffect;
    float lastHit = 0;
    void Update()
    {
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        DoRadiation();
        TurnAnimation();
    }

    void DoRadiation()
    {
        if (isDead) return;
        if (Time.time - lastHit < 1) return;
        Instantiate(radiationEffect, transform);
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                mob.TakeDamage(radiation, gameObject);
            }
        }
        lastHit = Time.time;
    }
}
