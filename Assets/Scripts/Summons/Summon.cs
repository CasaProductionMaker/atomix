using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Summon : NetworkBehaviour
{
    public string mobName;
    public float maxHealth;
    public float speed;
    public float bodyDamage;
    public float aggroRange = 10f;
    public NetworkVariable<float> health = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> GFXRotation = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Transform target;
    public Vector2 velocity;

    public Slider healthBar;
    public Transform GFX;
    public Electron parentElectron;

    protected void Start()
    {
        if (!IsOwner) return;
        health.Value = maxHealth;
        healthBar.value = health.Value / maxHealth;
    }

    public void TakeDamage(float damage)
    {
        health.Value -= damage;
    }

    public bool IsDead()
    {
        return health.Value <= 0;
    }

    public void MoveVector(Vector2 direction)
    {
        transform.Translate(direction);
    }

    public virtual void OnDamaged() {}

    public void DetectTarget()
    {
        if (target != null) return;
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, aggroRange);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                target = collider.transform;
                return;
            }
        }
    }

    public void RotateGFX()
    {
        GFX.rotation = Quaternion.Euler(0f, 0f, GFXRotation.Value);
    }

    public void MoveTowardsTarget()
    {
        if (target == null) return;
        Vector2 direction = (target.position - transform.position).normalized;
        velocity += direction * speed;
    }

    public void StayInBounds()
    {
        Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize + new Vector2(0.5f, 0.5f);
        float size = GetComponent<CircleCollider2D>().radius;
        bounds -= new Vector2(size, size);
        
        if(transform.position.x > bounds.x)
        {
            transform.position = new Vector3(bounds.x, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -bounds.x)
        {
            transform.position = new Vector3(-bounds.x, transform.position.y, transform.position.z);
        }
        if (transform.position.y > bounds.y)
        {
            transform.position = new Vector3(transform.position.x, bounds.y, transform.position.z);
        }
        if (transform.position.y < -bounds.y)
        {
            transform.position = new Vector3(transform.position.x, -bounds.y, transform.position.z);
        }
    }

    public void DieIfDead()
    {
        if (IsDead())
        {
            parentElectron.SummonDied();
            DestroySelfServerRpc();
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void DestroySelfServerRpc()
    {
        Destroy(gameObject);
    }

    public void CheckCollisions()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Summon summon))
            {
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                velocity -= hitAngle;
            }

            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                velocity -= hitAngle;
                mob.TakeDamageServerRpc(bodyDamage);
                TakeDamage(mob.bodyDamage);
                OnDamaged();
            }
        }
    }

    public void TickVelocity()
    {
        transform.position += (Vector3)velocity * Time.deltaTime;
        velocity *= 0.9f;

        StayInBounds();
    }

    public void UpdateHealthBar()
    {
        healthBar.value = health.Value / maxHealth;
    }
}
