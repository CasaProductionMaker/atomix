using System.Collections.Generic;
using UnityEngine;

public class Blade : Electron
{
    public float interval = 1f;
    Dictionary<Mob, float> damagedMobs = new Dictionary<Mob, float>();
    Dictionary<AntiElectron, float> damagedAntiElectrons = new Dictionary<AntiElectron, float>();
    
    void Update()
    {
        UpdateVisuals();
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
    }

    new void CheckCollisions()
    {
        if (isDead) return;
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, size);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                if (!damagedMobs.ContainsKey(mob) || Time.time - damagedMobs[mob] >= interval)
                {
                    damagedMobs[mob] = Time.time;
                    mob.TakeDamageServerRpc(damage);
                }
            }

            if (collider.gameObject.TryGetComponent(out AntiElectron antiElectron))
            {
                if (antiElectron.isDead) continue;
                if (!damagedAntiElectrons.ContainsKey(antiElectron) || Time.time - damagedAntiElectrons[antiElectron] >= interval)
                {
                    damagedAntiElectrons[antiElectron] = Time.time;
                    antiElectron.TakeDamageOwnerRpc(GetDamage());
                }
            }
        }
    }
}
