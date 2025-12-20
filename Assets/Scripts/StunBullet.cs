using UnityEngine;

public class StunBullet : Mob
{
    public GameObject stunPrefab;
    public Vector2 fixedVelocity;
    public float stunDuration;

    void Update()
    {
        velocity = fixedVelocity * speed;
        CheckCollisions();
        TickVelocity();
        DieIfDeadWithoutDecreasingCounter();
    }

    new void TickVelocity()
    {
        transform.position += (Vector3)velocity * Time.deltaTime;
        velocity *= 0.9f;

        DieIfOutOfBounds();
    }

    public override void OnDamaged()
    {
        // if (other.TryGetComponent(out Player player))
        // {
        //     StunEffect stunEffect = Instantiate(stunPrefab, player.transform.position, Quaternion.identity).GetComponent<StunEffect>();
        //     stunEffect.Initialize(stunDuration, player);
        // }
        // if (other.TryGetComponent(out Summon summon))
        // {
        //     StunEffect stunEffect = Instantiate(stunPrefab, player.transform.position, Quaternion.identity).GetComponent<StunEffect>();
        //     stunEffect.Initialize(stunDuration, summon);
        // }
    }
}