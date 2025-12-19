using UnityEngine;

public class Spiker : Mob
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
