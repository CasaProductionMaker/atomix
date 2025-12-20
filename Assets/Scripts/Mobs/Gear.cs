using UnityEngine;

public class Gear : Mob
{
    void Update()
    {
        UpdateHealthBar();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        DieIfDead();
    }
}