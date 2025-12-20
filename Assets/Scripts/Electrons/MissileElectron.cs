using UnityEngine;

public class MissileElectron : Electron
{
    public float missileSpeed = 10f;
    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        if (isDetached) MoveMissile();
        CheckCollisions();
    }

    void MoveMissile()
    {
        transform.Translate(Vector2.right * missileSpeed * Time.deltaTime);
        if (isOutOfBounds())
        {
            health = 0;
        }
    }
}
