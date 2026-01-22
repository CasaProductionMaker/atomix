using UnityEngine;

public class NormalAntiElectron : AntiElectron
{
    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        TurnAnimation();
    }
}
