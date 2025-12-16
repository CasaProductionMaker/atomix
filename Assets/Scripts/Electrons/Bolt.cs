using UnityEngine;

public class Bolt : Electron
{
    public float lightningDamage;
    public float zapRange;
    public GameObject lightningEffectPrefab;
    void Update()
    {
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        ZapIfPossible();
        TurnAnimation();
    }

    void ZapIfPossible()
    {
        if (isDead) return;
        if (GetTimeSinceDied() < reload + secondaryReload) return;
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, zapRange);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                // Summon lightning effect
                mob.TakeDamage(lightningDamage, gameObject);
                GameObject lightningEffect = Instantiate(lightningEffectPrefab, mob.transform.position, Quaternion.identity);
                lightningEffect.GetComponent<LightningEffect>().Initialize(transform.position, mob.transform.position);
                health = 0;
                return;
            }
        }
    }
}
