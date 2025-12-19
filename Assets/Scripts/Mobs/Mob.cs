using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Mob : NetworkBehaviour
{
    public string mobName;
    public float maxHealth;
    public float speed;
    public float bodyDamage;
    public float aggroRange = 10f;
    public NetworkVariable<float> health = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Transform target;
    public Vector2 velocity;
    public bool randomRotation = true;
    public MobDrop[] drops;
    public GameObject electronDropPrefab;

    public Slider healthBar;
    public Transform GFX;

    protected void Start()
    {
        if (!IsOwner) return;
        if (randomRotation) GFX.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        health.Value = maxHealth;
        healthBar.value = health.Value / maxHealth;
    }

    public void TakeDamage(float damage, GameObject damager)
    {
        health.Value -= damage;
        OnDamaged(damager);
    }

    public bool IsDead()
    {
        return health.Value <= 0;
    }

    public void MoveVector(Vector2 direction)
    {
        transform.Translate(direction);
    }

    public void DetectTarget()
    {
        if (target != null) return;
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, aggroRange);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Player player))
            {
                target = collider.transform;
                return;
            }
        }
    }

    public void MoveTowardsTarget()
    {
        if (target == null) return;
        Vector2 direction = (target.position - transform.position).normalized;
        velocity += direction * speed;
    }

    public void DieIfDead()
    {
        if (IsDead())
        {
            foreach (MobDrop drop in drops)
            {
                if (Random.value <= drop.spawnChance)
                {
                    GameObject dropSpawned = Instantiate(electronDropPrefab, transform.position, Quaternion.identity);
                    ElectronDrop electronDrop = dropSpawned.GetComponent<ElectronDrop>();
                    electronDrop.spreadDirection = Random.insideUnitCircle.normalized;
                    electronDrop.electronPrefab = drop.dropPrefab;
                }
            }

            FindFirstObjectByType<MobSpawner>().mobsLeftInWave--;

            Destroy(gameObject);
        }
    }

    public void DieIfDeadWithoutDrops()
    {
        if (IsDead())
        {
            FindFirstObjectByType<MobSpawner>().mobsLeftInWave--;

            Destroy(gameObject);
        }
    }

    public void DieIfDeadWithoutDecreasingCounter()
    {
        if (IsDead())
        {
            Destroy(gameObject);
        }
    }

    public void CheckCollisions()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                velocity -= hitAngle;
            }
        }
    }

    public virtual void OnDamaged(GameObject hitObject = null)
    {
        // Nothing by default
    }

    public void TickVelocity()
    {
        transform.position += (Vector3)velocity * Time.deltaTime;
        velocity *= 0.9f;

        StayInBounds();
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

    public void DieIfOutOfBounds()
    {
        Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize + new Vector2(0.5f, 0.5f);
        float size = GetComponent<CircleCollider2D>().radius;
        bounds -= new Vector2(size, size);

        if(transform.position.x > bounds.x || transform.position.x < -bounds.x || transform.position.y > bounds.y || transform.position.y < -bounds.y)
        {
            health.Value = 0;
        }
    }

    public void ApplyVelocity(Vector2 appliedVelocity)
    {
        velocity += appliedVelocity;
    }

    public void UpdateHealthBar()
    {
        healthBar.value = health.Value / maxHealth;
    }
}

[System.Serializable]
public struct MobDrop
{
    public GameObject dropPrefab;
    public float spawnChance;
}