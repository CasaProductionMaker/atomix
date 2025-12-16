using UnityEngine;

public class Slicer : Mob
{
    void Update()
    {
        DetectTarget();
        MoveTowardsTarget();
        if (target != null)
        {
            PlaySpinAnimation();
        }
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
    }

    void PlaySpinAnimation()
    {
        GFX.Rotate(0, 0, -750f * Time.deltaTime);
    }
}
