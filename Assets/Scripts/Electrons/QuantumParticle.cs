using UnityEngine;

public class QuantumParticle : Electron
{
    public float selfDamage;
    public float push = 10f;
    void Update()
    {
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
