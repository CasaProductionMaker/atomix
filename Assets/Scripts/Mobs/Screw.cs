using UnityEngine;

public class Screw : Mob
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
        GFX.Rotate(0, 0, -400f * Time.deltaTime);
    }
}
