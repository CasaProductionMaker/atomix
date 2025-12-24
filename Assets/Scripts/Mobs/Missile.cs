using UnityEngine;

public class Missile : Mob
{
    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        if (!IsOwner) return;
        DetectTarget();
        MoveTowardsTarget();
        LookAtTarget();
        CheckCollisions();
        TickVelocity();
        DieIfDead();
    }

    void LookAtTarget()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GFXRotation.Value = angle;
    }
}
