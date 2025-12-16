using UnityEngine;

public class BallBearing : Mob
{
    void Update()
    {
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
    }
}
