using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class Solder : Electron
{
    public float heal;
    bool isHealing = false;
    void Update()
    {
        DieIfDead();
        if (isDead) return;
        HealIfPossible();
        HealAnimation();
        CheckCollisions();
        TurnAnimation();
    }

    void HealIfPossible()
    {
        if (isDead) return;
        if (GetTimeSinceDied() < reload + secondaryReload) return;
        Player playerScript = player.GetComponent<Player>();
        if (playerScript.getHealth() >= playerScript.getMaxHealth()) return;
        isHealing = true;
        isDetached = true;
    }

    void HealAnimation()
    {
        if (!isHealing) return;

        if (DistanceToPlayer() > 0.1f)
        {
            transform.position = Vector2.Lerp(transform.position, player.transform.position, 0.2f);
        }
        else
        {
            PerformHeal();
        }
    }

    void PerformHeal()
    {

        Player playerScript = player.GetComponent<Player>();
        if (playerScript.getHealth() >= playerScript.getMaxHealth()) {
            isHealing = false;
            isDetached = false;
            return;
        }
        playerScript.Heal(heal);
        health = 0;
        isHealing = false;
    }

    float DistanceToPlayer()
    {
        return Vector2.Distance(transform.position, player.transform.position);
    }
}
