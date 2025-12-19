using UnityEngine;

public class Gear : Mob
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