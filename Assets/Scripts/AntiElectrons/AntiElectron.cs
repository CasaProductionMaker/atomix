using Unity.Netcode;
using UnityEngine;

public class AntiElectron : NetworkBehaviour
{
    public int electronID;
    public float maxHealth;
    public float damage;
    public float size;
    public float reload;
    public float secondaryReload;
    public float health;
    public float timeDied;
    public bool isDead;
    public bool isDetached;
    public bool doesExpand = true;
    public AntiAtom antiAtom;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDead = true;
        timeDied = Time.time;
        GetComponent<CircleCollider2D>().radius = size;
    }

    public void DieIfDead()
    {
        if (!antiAtom)
        {
            Destroy(gameObject);
            return;
        }
        if(health <= 0 && !isDead)
        {
            timeDied = Time.time;
            isDead = true;
            isDetached = false;
        }
        if (isDead)
        {
            if (GetTimeSinceDied() >= reload)
            {
                health = maxHealth;
                isDead = false;
            }
        }
        SetGFXEnabledRpc(!isDead);
    }

    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetGFXEnabledRpc(bool isEnabled)
    {
        GetComponent<SpriteRenderer>().enabled = isEnabled;
    }

    public float GetTimeSinceDied()
    {
        return Time.time - timeDied;
    }

    public void CheckCollisions()
    {
        if (isDead) return;
        
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, size);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Player player))
            {
                player.TakeDamageOwnerRpc(GetDamage());
                health -= player.getBodyDamage();
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                float totalDistance = size + (collider as CircleCollider2D).radius;
                float multiplier = totalDistance - (collider.transform.position - transform.position).magnitude;
                transform.position += (Vector3)(-hitAngle * multiplier);
                player.ApplyVelocityOwnerRpc(hitAngle * 0.02f);
            }
        }
    }

    public void TurnAnimation()
    {
        transform.Rotate(0f, 0f, 5f * Time.deltaTime);
    }

    public bool isOutOfBounds()
    {
        Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize + new Vector2(0.5f, 0.5f);
        return Mathf.Abs(transform.position.x) + size > bounds.x || Mathf.Abs(transform.position.y) + size > bounds.y;
    }
    public void StayInBounds()
    {
        Vector3 pos = transform.position;
        Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize + new Vector2(0.5f, 0.5f);
        pos.x = Mathf.Clamp(pos.x, size - bounds.x, bounds.x - size);
        pos.y = Mathf.Clamp(pos.y, size - bounds.y, bounds.y - size);
        transform.position = pos;
    }

    public virtual float GetDamage()
    {
        float returnDamage = damage;

        return returnDamage;
    }

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Everyone)]
    public void TakeDamageOwnerRpc(float damageAmount)
    {
        health -= damageAmount;
    }

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Everyone)]
    public void ApplyVelocityOwnerRpc(Vector2 velocity)
    {
        transform.position += (Vector3)velocity;
    }
}
