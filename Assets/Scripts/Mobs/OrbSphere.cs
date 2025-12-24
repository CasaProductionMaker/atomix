using UnityEngine;

public class OrbSphere : Mob
{
    public float heal;
    float lastHealTime = 0f;

    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        Heal();
        DieIfDead();
    }

    void Heal()
    {
        if (Time.time - lastHealTime < 1f) return;

        lastHealTime = Time.time;
        health.Value += heal;
        if (health.Value > maxHealth) health.Value = maxHealth;
    }
}
