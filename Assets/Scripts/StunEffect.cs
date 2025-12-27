using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class StunEffect : NetworkBehaviour
{
    public NetworkVariable<float> duration = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Mob mob;
    public Player player;
    public Summon summon;
    ParticleSystem ps;
    float startTime;
    bool startedPlaying;
    void Start()
    {
        startedPlaying = false;
    }
    public void Initialize(float duration, Mob targetMob)
    {
        transform.position = targetMob.transform.position;
        this.duration.Value = duration;
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
        this.duration.Value = duration;
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
        this.duration.Value = duration;
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
        Debug.Log(startedPlaying);
        if (!startedPlaying && !IsOwner)
        {
            startedPlaying = true;
            ps = GetComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = duration.Value;
            ps.Play();
        }

        if (!IsOwner) return;

        if (mob == null && player == null && summon == null)
        {
            Destroy(gameObject);
            return;
        } else {
            if (mob != null) {
                if (Time.time - startTime <= duration.Value) mob.transform.position = transform.position;
            } else if (player != null){
                if (Time.time - startTime <= duration.Value) player.transform.position = transform.position;
            } else {
                if (Time.time - startTime <= duration.Value) summon.transform.position = transform.position;
            }
        }
    }
}
