using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    Vector2 velocity;
    [SerializeField] float speed = 5f;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float health;
    [SerializeField] float bodyDamage = 3f;
    public GameObject deathScreen;
    bool isDead = false;

    // Inputs
    public InputActionReference moveInput;

    // UI
    public Slider healthBar;

    // References
    public PlayerElectronController electronController;

    // Effects
    public GameObject nucleusPopEffect;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        electronController = GetComponent<PlayerElectronController>();
        health = maxHealth;
        healthBar.value = health / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        velocity += moveInput.action.ReadValue<Vector2>().normalized * speed;

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                mob.TakeDamage(bodyDamage, gameObject);
                health -= mob.bodyDamage;
                healthBar.value = health / getMaxHealth();
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                velocity -= hitAngle * 1.5f;
                mob.MoveVector(hitAngle * 0.001f);
            }
        }

        transform.position += (Vector3)velocity * Time.deltaTime;
        velocity *= 0.9f;

        Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize;
        if (transform.position.y < -bounds.y)
        {
            transform.position = new Vector3(transform.position.x, -bounds.y, transform.position.z);
        }
        if (transform.position.y > bounds.y)
        {
            transform.position = new Vector3(transform.position.x, bounds.y, transform.position.z);
        }
        if (transform.position.x < -bounds.x)
        {
            transform.position = new Vector3(-bounds.x, transform.position.y, transform.position.z);
        }
        if (transform.position.x > bounds.x)
        {
            transform.position = new Vector3(bounds.x, transform.position.y, transform.position.z);
        }

        if (getHealth() <= 0f)
        {
            Nucleus activeNucleus = electronController.GetActiveNucleus();
            if (activeNucleus != null)
            {
                health = getMaxHealth();
                healthBar.value = health / getMaxHealth();
                transform.position = activeNucleus.transform.position;
                Instantiate(nucleusPopEffect, activeNucleus.transform.position, Quaternion.identity);
                activeNucleus.health = 0;
                activeNucleus.DieIfDead();
                return;
            }

            deathScreen.SetActive(true);
            bodyDamage = 0f;
            electronController.KillAllElectrons();
            isDead = true;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }

    public void ApplyVelocity(Vector2 appliedVelocity)
    {
        velocity += appliedVelocity;
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > getMaxHealth())
        {
            health = getMaxHealth();
        }
        healthBar.value = health / getMaxHealth();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.value = health / getMaxHealth();
    }

    public float getHealth()
    {
        return health;
    }
    public float getMaxHealth()
    {
        float additionalHealth = electronController.GetHealthBonus();
        return maxHealth + additionalHealth;
    }
}
