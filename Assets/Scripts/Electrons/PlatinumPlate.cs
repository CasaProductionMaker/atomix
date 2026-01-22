using UnityEngine;

public class PlatinumPlate : Electron
{
    public float defense = 2f;
    public float damageReduction = 0.05f;
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
