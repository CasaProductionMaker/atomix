using UnityEngine;

public class HeavyElectron : Electron
{
    public float distanceFromPlayer = 5f;
    void Update()
    {
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        if (isDetached) CastHeavyElectron();
        StayInBounds();
        CheckCollisions();
        TurnAnimation();
    }

    void CastHeavyElectron()
    {
        float multiplier = Vector2.Distance(player.transform.position, transform.position) - distanceFromPlayer;
        transform.position += new Vector3(
            (player.transform.position.x - transform.position.x) / 40 * multiplier, 
            (player.transform.position.y - transform.position.y) / 40 * multiplier, 
            0
        );
    }
}
