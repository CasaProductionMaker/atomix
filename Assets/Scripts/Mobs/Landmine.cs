using UnityEngine;
using TMPro;

public class Landmine : Mob
{
    public float explosionDamage = 80f;
    public float explosionRadius = 3f;
    public float explosionTimer = 3f;
    public GameObject explosionEffect;
    public TextMeshProUGUI timerText;
    float timeHit = 0;
    bool wasHit = false;

    void Update()
    {
        UpdateHealthBar();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        UpdateTimerText();
        ExplodeIfNeeded();
        DieIfDead();
    }

    public override void OnDamaged()
    {
        if (wasHit) return;
        timeHit = Time.time;
        wasHit = true;
    }

    void UpdateTimerText()
    {
        if (!wasHit)
        {
            timerText.text = "0:03";
            return;
        }
        float timeLeft = explosionTimer - (Time.time - timeHit);
        if (timeLeft < 0) timeLeft = 0;
        int seconds = Mathf.CeilToInt(timeLeft);
        timerText.text = "0:" + seconds.ToString("00");
    }

    void ExplodeIfNeeded()
    {
        if (!wasHit) return;
        if (Time.time - timeHit < explosionTimer) return;
        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explosion.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach(Collider2D collider in hit)
        {
            if(collider.TryGetComponent(out Player player))
            {
                player.TakeDamage(explosionDamage);
            }
        }
        health.Value = 0;
        DieIfDeadWithoutDrops();
    }
}
