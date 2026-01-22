using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using NUnit.Framework;

public class Electron : NetworkBehaviour
{
    public int electronID;
    public string electronName;
    public string description;
    public string actualDescription;
    [TextArea] public string stats;
    [Header("Electron Stats")]
    public float maxHealth;
    public float damage;
    public float size;
    public float reload;
    public float secondaryReload;
    public float health;
    public float timeDied;
    public bool isDead;
    public bool isDetached;
    public bool doesExpand = true;
    public bool unstackable = false;
    public PlayerElectronController player;
    public SpawnedElectron spawnedElectronInfo;
    public bool isPlayerDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDead = true;
        timeDied = Time.time;
        GetComponent<CircleCollider2D>().radius = size;
    }

    public void UpdateVisuals()
    {
        //GetComponent<SpriteRenderer>().enabled = !isDead;
    }

    public void DieIfDead()
    {
        if (isPlayerDead) health = 0;
        if(health <= 0 && !isDead)
        {
            timeDied = Time.time;
            isDead = true;
            isDetached = false;
        }
        if (isDead)
        {
            if (GetTimeSinceDied() >= reload)
            {
                health = maxHealth;
                isDead = false;
            }
        }
        SetGFXEnabledRpc(!isDead);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetGFXEnabledRpc(bool isEnabled)
    {
        GetComponent<SpriteRenderer>().enabled = isEnabled;
    }

    public float GetTimeSinceDied()
    {
        return Time.time - timeDied;
    }

    public void CheckCollisions()
    {
        if (isDead) return;
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, size);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                if (mob is Phaser phaser)
                {
                    if (Random.Range(0f, 1f) < phaser.evasionChance) continue;
                }

                mob.TakeDamageServerRpc(GetDamage());
                health -= mob.bodyDamage;
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                float totalDistance = size + (collider as CircleCollider2D).radius;
                float multiplier = totalDistance - (collider.transform.position - transform.position).magnitude;
                transform.position += (Vector3)(-hitAngle * multiplier);
                mob.MoveVectorServerRpc(hitAngle * 0.02f);

                Neutralizer neutralizer = player.GetActiveNeutralizer();
                if (neutralizer != null)
                {
                    mob.transform.position = neutralizer.transform.position;
                }
            }

            if (collider.gameObject.TryGetComponent(out AntiElectron antiElectron))
            {
                if (antiElectron.isDead) continue;
                antiElectron.TakeDamageOwnerRpc(GetDamage());
                health -= antiElectron.GetDamage();

                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                float totalDistance = size + (collider as CircleCollider2D).radius;
                float multiplier = totalDistance - (collider.transform.position - transform.position).magnitude;

                transform.position += (Vector3)(-hitAngle * multiplier);
                antiElectron.ApplyVelocityOwnerRpc(hitAngle * 0.02f);
            }
        }
    }

    public void TurnAnimation()
    {
        transform.Rotate(0f, 0f, 5f * Time.deltaTime);
    }

    public void SummonDied()
    {
        health = 0;
    }
    public bool isOutOfBounds()
    {
        Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize + new Vector2(0.5f, 0.5f);
        return Mathf.Abs(transform.position.x) + size > bounds.x || Mathf.Abs(transform.position.y) + size > bounds.y;
    }
    public void StayInBounds()
    {
        Vector3 pos = transform.position;
        Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize + new Vector2(0.5f, 0.5f);
        pos.x = Mathf.Clamp(pos.x, size - bounds.x, bounds.x - size);
        pos.y = Mathf.Clamp(pos.y, size - bounds.y, bounds.y - size);
        transform.position = pos;
    }

    public virtual float GetDamage()
    {
        float returnDamage = damage;

        // Loop over build for cores
        for (int i = 0; i < player.getMaxElectronsPerShell().Length; i++)
        {
            List<SpawnedElectron> electrons = player.electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                Electron electronReference = electron.electronReference;
                
                if (electronReference is Core core && spawnedElectronInfo.shell == 2)
                {
                    returnDamage *= core.damageMultiplier;
                }
            }
        }

        // Loop over build for platinum plates
        float finalDamageMultiplier = 1f;
        for (int i = 0; i < player.getMaxElectronsPerShell().Length; i++)
        {
            List<SpawnedElectron> electrons = player.electronsPerShell[i];
            foreach (SpawnedElectron electron in electrons)
            {
                if (electron.isEmptySlot) continue;
                Electron electronReference = electron.electronReference;
                
                if (electronReference is PlatinumPlate plate)
                {
                    finalDamageMultiplier -= plate.damageReduction;
                }
            }
        }
        if (finalDamageMultiplier < 0f) finalDamageMultiplier = 0f;
        returnDamage *= finalDamageMultiplier;

        return returnDamage;
    }
}
