using UnityEngine;

public class Slicer : Mob
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
        GFX.Rotate(0, 0, -750f * Time.deltaTime);
    }
}
