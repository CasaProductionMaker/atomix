using UnityEngine;

public class AntiSolder : AntiElectron
{
    public float heal;
    bool isHealing = false;
    void Update()
    {
        if (!IsOwner) return;
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
        AntiAtom antiAtomScript = antiAtom.GetComponent<AntiAtom>();
        if (antiAtomScript.health.Value >= antiAtomScript.getMaxHealth()) return;
        isHealing = true;
        isDetached = true;
    }

    void HealAnimation()
    {
        if (!isHealing) return;

        if (DistanceToAntiAtom() > 0.3f)
        {
            transform.position = Vector2.Lerp(transform.position, antiAtom.transform.position, 0.3f);
        }
        else
        {
            PerformHeal();
        }
    }

    void PerformHeal()
    {
        AntiAtom antiAtomScript = antiAtom.GetComponent<AntiAtom>();
        if (antiAtomScript.health.Value >= antiAtomScript.getMaxHealth()) {
            isHealing = false;
            isDetached = false;
            return;
        }
        antiAtomScript.health.Value += heal;
        health = 0;
        isHealing = false;
    }

    float DistanceToAntiAtom()
    {
        return Vector2.Distance(transform.position, antiAtom.transform.position);
    }
}
