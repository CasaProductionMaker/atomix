using UnityEngine;

public class Spiker : Mob
{
    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        DieIfDead();
    }
}
