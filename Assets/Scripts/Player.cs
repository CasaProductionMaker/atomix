using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Collections;
using TMPro;

public class Player : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> playerUsername = new NetworkVariable<FixedString64Bytes>("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    Vector2 velocity;
    [SerializeField] float speed = 5f;
    [SerializeField] float maxHealth = 100f;
    [SerializeField] NetworkVariable<float> health = new NetworkVariable<float>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] float bodyDamage = 3f;
    public GameObject deathScreen;
    bool isDead = false;

    // Inputs
    public InputActionReference moveInput;

    // UI
    public Slider healthBar;
    public TextMeshProUGUI usernameText;

    // References
    public PlayerElectronController electronController;
    public GameObject myCamera;

    // Effects
    public GameObject nucleusPopEffect;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!IsOwner) {
            Destroy(myCamera);
            return;
        }
        electronController = GetComponent<PlayerElectronController>();
        health.Value = maxHealth;
        healthBar.value = health.Value / maxHealth;
        deathScreen = FindFirstObjectByType<MobSpawner>().deathScreen;
        playerUsername.Value = new FixedString64Bytes(ChatManager.Singleton.username);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) {
            healthBar.value = health.Value / maxHealth;
            usernameText.text = playerUsername.Value.ToString();
            return;
        }

        deathScreen.SetActive(isDead);
        if (getHealth() > 0f)
        {
            isDead = false;
        }
        if (isDead) return;

        if (!ChatManager.Singleton.isChatSelected()) velocity += moveInput.action.ReadValue<Vector2>().normalized * speed;

        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Mob mob))
            {
                mob.TakeDamageServerRpc(bodyDamage);
                health.Value -= mob.bodyDamage;
                healthBar.value = health.Value / getMaxHealth();
                Vector2 hitAngle = (collider.transform.position - transform.position).normalized;
                velocity -= hitAngle * 1.5f;
                mob.ApplyVelocityServerRpc(hitAngle * 0.1f);
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
                health.Value = getMaxHealth();
                healthBar.value = health.Value / getMaxHealth();
                transform.position = activeNucleus.transform.position;
                Instantiate(nucleusPopEffect, activeNucleus.transform.position, Quaternion.identity);
                activeNucleus.health = 0;
                activeNucleus.DieIfDead();
                return;
            }

            bodyDamage = 0f;
            electronController.KillAllElectrons();
            isDead = true;
        }
    }

    public Vector2 GetVelocity()
    {
        return velocity;
    }

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Everyone)]
    public void ApplyVelocityOwnerRpc(Vector2 appliedVelocity)
    {
        velocity += appliedVelocity;
    }

    public void Heal(float amount)
    {
        health.Value += amount;
        if (health.Value > getMaxHealth())
        {
            health.Value = getMaxHealth();
        }
        healthBar.value = health.Value / getMaxHealth();
    }

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Everyone)]
    public void TakeDamageOwnerRpc(float damage)
    {
        health.Value -= damage;
        healthBar.value = health.Value / getMaxHealth();
    }

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Everyone)]
    public void ReviveOwnerRpc()
    {
        health.Value = getMaxHealth();
        electronController.ReviveAllElectrons();
        healthBar.value = health.Value / getMaxHealth();
    }

    public float getHealth()
    {
        return health.Value;
    }
    public float getMaxHealth()
    {
        float additionalHealth = electronController.GetHealthBonus();
        return maxHealth + additionalHealth;
    }
}
