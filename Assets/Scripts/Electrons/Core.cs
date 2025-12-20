using UnityEngine;

public class Core : Electron
{
    public float damageMultiplier;
    public GameObject effects;
    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        EffectsLogic();
        if (isDead) return;
        CheckCollisions();
        TurnAnimation();
    }

    void EffectsLogic()
    {
        effects.SetActive(!isDead);
    }
}
