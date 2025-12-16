using UnityEngine;

public class PoisonEffect : MonoBehaviour
{
    public float duration;
    public float damagePerSecond;
    public Mob mob;
    float lastDamageTime;
    ParticleSystem ps;
    public void Initialize(float deterioration, float duration, Mob targetMob)
    {
        damagePerSecond = deterioration;
        this.duration = duration;
        mob = targetMob;
        lastDamageTime = Time.time;
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = duration;
        ps.Play();
        Destroy(gameObject, duration + 1f);
    }
    // Update is called once per frame
    void Update()
    {
        if (mob == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = mob.transform.position;
        if (Time.time - lastDamageTime >= 1f)
        {
            mob.TakeDamage(damagePerSecond, gameObject);
            lastDamageTime = Time.time;
        }
    }
}
