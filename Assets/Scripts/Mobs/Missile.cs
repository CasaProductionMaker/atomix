using UnityEngine;

public class Missile : Mob
{
    void Update()
    {
        DetectTarget();
        MoveTowardsTarget();
        LookAtTarget();
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
    }

    void LookAtTarget()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GFX.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
