using UnityEngine;

public class AntiRawPlatinum : AntiElectron
{
    public float distanceFromAtom = 3f;
    public float speed = 10f;
    void Update()
    {
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
        float multiplier = Vector2.Distance(antiAtom.transform.position, transform.position) - distanceFromAtom;
        transform.position += new Vector3(
            (antiAtom.transform.position.x - transform.position.x) * multiplier, 
            (antiAtom.transform.position.y - transform.position.y) * multiplier, 
            0
        ).normalized * Time.deltaTime * speed;
    }
}
