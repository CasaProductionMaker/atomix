using Unity.Netcode;
using UnityEngine;

public class StunBullet : Mob
{
    public GameObject stunPrefab;
    public Vector2 fixedVelocity;
    public float stunDuration;

    void Update()
    {
        velocity = fixedVelocity * speed;
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
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius * 1.05f);
        foreach (Collider2D collider in hit)
        {
            if (collider.TryGetComponent(out Player player))
            {
                StunEffect stunEffect = Instantiate(stunPrefab, player.transform.position, Quaternion.identity).GetComponent<StunEffect>();
                stunEffect.gameObject.GetComponent<NetworkObject>().Spawn(true);
                stunEffect.Initialize(stunDuration, player);
            }
            if (collider.TryGetComponent(out Summon summon))
            {
                StunEffect stunEffect = Instantiate(stunPrefab, player.transform.position, Quaternion.identity).GetComponent<StunEffect>();
                stunEffect.gameObject.GetComponent<NetworkObject>().Spawn(true);
                stunEffect.Initialize(stunDuration, summon);
            }
        }
    }
}