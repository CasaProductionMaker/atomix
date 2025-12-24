using UnityEngine;

public class Tanker : Mob
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
