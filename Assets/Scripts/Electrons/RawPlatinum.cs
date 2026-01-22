using UnityEngine;

public class RawPlatinum : Electron
{
    public float distanceFromPlayer = 3f;
    public float speed = 10f;
    void Update()
    {
        UpdateVisuals();
        if (!IsOwner) return;
        DieIfDead();
        if (isDead) return;
        CastRawPlatinum();
        StayInBounds();
        CheckCollisions();
        TurnAnimation();
    }

    void CastRawPlatinum()
    {
        isDetached = true;
        float multiplier = Vector2.Distance(player.transform.position, transform.position) - distanceFromPlayer;
        transform.position += new Vector3(
            (player.transform.position.x - transform.position.x) * multiplier, 
            (player.transform.position.y - transform.position.y) * multiplier, 
            0
        ).normalized * Time.deltaTime * speed;
    }
}
