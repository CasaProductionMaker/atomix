using UnityEngine;

public class Phaser : Mob
{
    public float evasionChance = 0.9f;
    public int state = 0; // 0 = chasing, 1 = circling, 2 = charging
    public float circlingRadius = 3f;
    float stateSwitchTime = 0f;
    float circlingDuration = 3f;

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

    public override void MoveTowardsTarget()
    {
        if (target == null) return;

        switch (state)
        {
            case 0: // chasing
                {
                    Vector2 direction = (target.position - transform.position).normalized;
                    velocity += direction * speed;
                    
                    if (Vector2.Distance(transform.position, target.position) < circlingRadius)
                    {
                        state = 1;
                        stateSwitchTime = Time.time;
                    }
                    break;
                }
            case 1: // circling
                {
                    Vector2 toTarget = target.position - transform.position;
                    Vector2 perpendicular = new Vector2(-toTarget.y, toTarget.x).normalized;
                    velocity += perpendicular * speed;
                    
                    if (Time.time - stateSwitchTime >= circlingDuration)
                    {
                        state = 2;
                    }
                    break;
                }
            case 2: // charging
                {
                    Vector2 direction = (target.position - transform.position).normalized;
                    velocity += direction * speed * 2f;
                    break;
                }
        }
    }

    void LookAtTarget()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GFXRotation.Value = angle;
    }
}
