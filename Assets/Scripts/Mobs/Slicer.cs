using UnityEngine;

public class Slicer : Mob
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
        GFXRotation.Value -= 750 * Time.deltaTime;
    }
}
