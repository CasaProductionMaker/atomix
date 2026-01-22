using UnityEngine;

public class Nucleus : Electron
{
    void Update()
    {
        UpdateVisuals();
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        StayInBounds();
        CheckCollisions();
        TurnAnimation();
    }

    public bool isActive()
    {
        return Time.time - timeDied >= reload + secondaryReload && isDetached;
    }
}
