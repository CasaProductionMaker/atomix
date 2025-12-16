using UnityEngine;

public class Tanker : Mob
{
    void Update()
    {
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
    }
}
