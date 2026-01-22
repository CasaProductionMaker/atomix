using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.UIElements;

public class AntiAtom : Mob
{
    public float circlingRadius = 3f;
    List<GameObject> heldElectrons = new List<GameObject>();
    float electronAngle = 0f;
    float electronDistance = 2f;
    public SpawnableAntiElectron[] antiElectronPrefabs;

    new void Start()
    {
        base.Start();
        float electronAmount = Mathf.Floor((FindFirstObjectByType<MobSpawner>().waveNumber.Value - 20) / 5);
        List<int> usedIndices = new List<int>();
        for (int i = 0; i < electronAmount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, antiElectronPrefabs.Length);
            while (antiElectronPrefabs[randomIndex].isStackable == false && usedIndices.Contains(randomIndex)) // do i have one yet?
            {
                randomIndex = UnityEngine.Random.Range(0, antiElectronPrefabs.Length);
            }
            GameObject electron = Instantiate(antiElectronPrefabs[randomIndex].electronPrefab, transform.position, Quaternion.identity);
            electron.GetComponent<NetworkObject>().Spawn();
            AntiElectron antiElectronScript = electron.GetComponent<AntiElectron>();
            antiElectronScript.antiAtom = this;
            heldElectrons.Add(electron);
            usedIndices.Add(randomIndex);
        }
    }

    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        if (!IsOwner) return;
        DetectTarget();
        MoveTowardsTarget();
        RotateElectrons();
        CheckCollisions();
        TickVelocity();
        DieIfDead();
    }

    public new void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = health.Value / getMaxHealth();
        }
    }

    public override void MoveTowardsTarget()
    {
        if (GetActiveRawPlatinum() != null)
        {
            velocity += new Vector2(1, 1).normalized * speed;
            return;
        }

        if (target == null) return;
        
        if (Vector2.Distance(transform.position, target.position) > circlingRadius)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            velocity += direction * speed;
        } else {
            Vector2 toTarget = target.position - transform.position;
            Vector2 perpendicular = new Vector2(-toTarget.y, toTarget.x).normalized;
            velocity += perpendicular * speed;
        }
    }

    void RotateElectrons()
    {
        float degreesPerElectron = 2f * (float)Math.PI / heldElectrons.Count;
        electronAngle += Time.deltaTime * 3f;

        int i = 0;
        foreach (GameObject electron in heldElectrons)
        {
            if (electron.GetComponent<AntiElectron>().isDead) 
            {
                electron.transform.position = transform.position;
                i++;
                continue; 
            }

            if (!electron.GetComponent<AntiElectron>().isDetached) {
                float targetAngle = electronAngle + degreesPerElectron * i;
                AntiRawPlatinum rawPlatinum = GetActiveRawPlatinum();
                float targetDistance = electronDistance;
                if (!electron.GetComponent<AntiElectron>().doesExpand)
                {
                    targetDistance -= 1f;
                }
                if (rawPlatinum != null)
                {
                    targetDistance += rawPlatinum.size;
                }
                Vector2 targetPosition;
                if (rawPlatinum != null)
                {
                    targetPosition = new Vector2((float)Math.Sin(targetAngle) * targetDistance, (float)Math.Cos(targetAngle) * targetDistance) + (Vector2)rawPlatinum.transform.position;
                } else {
                    targetPosition = new Vector2((float)Math.Sin(targetAngle) * targetDistance, (float)Math.Cos(targetAngle) * targetDistance) + (Vector2)transform.position;
                }
                float easing = 20f;
                electron.transform.position = Vector2.Lerp(electron.transform.position, targetPosition, easing * Time.deltaTime);
            }

            i++;
        }
    }

    AntiRawPlatinum GetActiveRawPlatinum()
    {
        foreach (GameObject electron in heldElectrons)
        {
            if (electron.TryGetComponent(out AntiRawPlatinum antiRawPlatinum) && !antiRawPlatinum.isDead)
            {
                return antiRawPlatinum.GetComponent<AntiRawPlatinum>();
            }
        }

        return null;
    }

    public float getMaxHealth()
    {
        float returnMaxHealth = maxHealth;
        foreach (GameObject electron in heldElectrons)
        {
            if (electron.TryGetComponent(out AntiNeutron antiNeutron))
            {
                returnMaxHealth += antiNeutron.maxHealthBonus;
            }
        }

        return returnMaxHealth;
    }
}

[Serializable]
public struct SpawnableAntiElectron
{
    public GameObject electronPrefab;
    public bool isStackable;
}