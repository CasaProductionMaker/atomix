using System.Collections;
using UnityEngine;

public class StunEffect : MonoBehaviour
{
    public float duration;
    public Mob mob;
    public Player player;
    public Summon summon;
    ParticleSystem ps;
    float startTime;
    public void Initialize(float duration, Mob targetMob)
    {
        transform.position = targetMob.transform.position;
        this.duration = duration;
        mob = targetMob;
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = duration;
        ps.Play();
        startTime = Time.time;
        Destroy(gameObject, duration + 1f);
    }
    public void Initialize(float duration, Player targetPlayer)
    {
        transform.position = targetPlayer.transform.position;
        this.duration = duration;
        player = targetPlayer;
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = duration;
        ps.Play();
        startTime = Time.time;
        Destroy(gameObject, duration + 1f);
    }
    public void Initialize(float duration, Summon targetSummon)
    {
        transform.position = targetSummon.transform.position;
        this.duration = duration;
        summon = targetSummon;
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = duration;
        ps.Play();
        startTime = Time.time;
        Destroy(gameObject, duration + 1f);
    }
    // Update is called once per frame
    void Update()
    {
        if (mob == null && player == null && summon == null)
        {
            Destroy(gameObject);
            return;
        } else {
            if (mob != null) {
                if (Time.time - startTime <= duration) mob.transform.position = transform.position;
            } else if (player != null){
                if (Time.time - startTime <= duration) player.transform.position = transform.position;
            } else {
                if (Time.time - startTime <= duration) summon.transform.position = transform.position;
            }
        }
    }
}
