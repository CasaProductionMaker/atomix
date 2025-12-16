using UnityEngine;

public class Neutron : Electron
{
    public float maxHealthBonus = 200f;
    void Update()
    {
        DieIfDead();
        if (isDead) return;
        CheckCollisions();
        TurnAnimation();
    }
}
