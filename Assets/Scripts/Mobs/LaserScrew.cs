using UnityEngine;

public class LaserScrew : Mob
{
    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        if (!IsOwner) return;
        DetectTarget();
        MoveTowardsTarget();
        if (target != null)
        {
            PlaySpinAnimation();
        }
        CheckCollisions();
        TickVelocity();
        DieIfDead();
    }

    void PlaySpinAnimation()
    {
        GFXRotation.Value -= 500f * Time.deltaTime;
    }
}
