using UnityEngine;

public class LaserScrew : Mob
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
        GFX.Rotate(0, 0, -500f * Time.deltaTime);
    }
}
