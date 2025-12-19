using UnityEngine;

public class Tanker : Mob
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
