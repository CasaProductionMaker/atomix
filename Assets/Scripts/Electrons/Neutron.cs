using UnityEngine;

public class Neutron : Electron
{
    public float maxHealthBonus = 200f;
    void Update()
    {
        UpdateVisuals();
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        TurnAnimation();
    }
}
