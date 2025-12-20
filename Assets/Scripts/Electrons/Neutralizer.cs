using UnityEngine;

public class Neutralizer : Electron
{
    public GameObject teleportEffect;

    void Update()
    {
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
