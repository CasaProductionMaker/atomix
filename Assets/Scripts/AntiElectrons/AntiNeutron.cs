using UnityEngine;

public class AntiNeutron : AntiElectron
{
    public float maxHealthBonus = 200f;
    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        TurnAnimation();
    }
}
