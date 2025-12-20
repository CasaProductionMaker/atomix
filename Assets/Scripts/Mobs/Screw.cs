using UnityEngine;

public class Screw : Mob
{
    void Update()
    {
        UpdateHealthBar();
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
        GFX.Rotate(0, 0, -400f * Time.deltaTime);
    }
}
