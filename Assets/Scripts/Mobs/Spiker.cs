using UnityEngine;

public class Spiker : Mob
{
    void Update()
    {
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
    }
}
