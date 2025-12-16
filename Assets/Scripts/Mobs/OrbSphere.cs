using UnityEngine;

public class OrbSphere : Mob
{
    public float heal;
    float lastHealTime = 0f;

    void Update()
    {
        CheckCollisions();
        TickVelocity();
        Heal();
        UpdateHealthBar();
        DieIfDead();
    }

    void Heal()
    {
        if (Time.time - lastHealTime < 1f) return;

        lastHealTime = Time.time;
        health += heal;
        if (health > maxHealth) health = maxHealth;
    }
}
