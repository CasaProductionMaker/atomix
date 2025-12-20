using UnityEngine;

public class Tanker : Mob
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
