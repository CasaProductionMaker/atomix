using UnityEngine;

public class NormalElectron : Electron
{
    void Update()
    {
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        TurnAnimation();
    }
}
