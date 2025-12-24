using Unity.Netcode;
using UnityEngine;

public class PoisonEffect : NetworkBehaviour
{
    public NetworkVariable<float> duration = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float damagePerSecond;
    public Mob mob;
    float lastDamageTime;
    public ParticleSystem ps;
    bool startedPlaying = false;

    void Start()
    {
        startedPlaying = false;
    }
    public void Initialize(float deterioration, float duration, Mob targetMob)
    {
        damagePerSecond = deterioration;
        this.duration.Value = duration;
        mob = targetMob;
        lastDamageTime = Time.time;
        var main = ps.main;
        main.duration = duration;
        ps.Play();
        Destroy(gameObject, duration + 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedPlaying && !IsOwner)
        {
            startedPlaying = true;
            var main = ps.main;
            main.duration = duration.Value;
            ps.Play();
        }

        if (!IsOwner) return;

        if (mob == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = mob.transform.position;
        if (Time.time - lastDamageTime >= 1f)
        {
            mob.TakeDamageServerRpc(damagePerSecond);
            lastDamageTime = Time.time;
        }
    }
}
