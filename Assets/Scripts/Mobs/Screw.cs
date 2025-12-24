using UnityEngine;

public class Screw : Mob
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
        GFXRotation.Value -= 400f * Time.deltaTime;
    }
}
