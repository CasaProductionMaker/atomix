using UnityEngine;

public class BallBearing : Mob
{
    void Update()
    {
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
    }
}
