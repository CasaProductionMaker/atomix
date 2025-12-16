using UnityEngine;

public class Anomaly : Mob
{
    void Update()
    {
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
    }
}
