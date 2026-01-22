using UnityEngine;

public class NormalElectron : Electron
{
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
