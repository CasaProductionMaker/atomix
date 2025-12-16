using UnityEngine;

public class Gear : Mob
{
    void Update()
    {
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
    }
}