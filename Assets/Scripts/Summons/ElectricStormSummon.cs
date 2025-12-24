using UnityEngine;

public class ElectricStormSummon : Summon
{
    float currentSpeed = 0f;
    Vector2 randomAngle;
    float targetResetInterval = 0f;
    float lastTargetResetTime = 0f;

    new void Start()
    {
        ChooseRandomTarget();
        base.Start();
    }

    void Update()
    {
        RotateGFX();
        UpdateHealthBar();
        if (!IsOwner) return;
        PlaySpinAnimation();
        if (Time.time - lastTargetResetTime >= targetResetInterval) ChooseRandomTarget();
        MoveTowardsRandomTarget();
        CheckCollisions();
        TickVelocity();
        DieIfDead();
        DieIfSpawnerDead();
    }

    void PlaySpinAnimation()
    {
        GFXRotation.Value -= 1000f * Time.deltaTime;
    }

    void ChooseRandomTarget()
    {
        randomAngle = Random.insideUnitCircle.normalized;
        currentSpeed = Random.Range(speed * 0.1f, speed);
        targetResetInterval = Random.Range(0.5f, 2f);
        lastTargetResetTime = Time.time;
    }

    void MoveTowardsRandomTarget()
    {
        velocity += randomAngle * currentSpeed;
    }

    void DieIfSpawnerDead()
    {
        if (!parentElectron)
        {
            DestroySelfServerRpc();
        }
    }
}
