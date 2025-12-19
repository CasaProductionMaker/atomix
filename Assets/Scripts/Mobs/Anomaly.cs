using UnityEngine;

public class Anomaly : Mob
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
