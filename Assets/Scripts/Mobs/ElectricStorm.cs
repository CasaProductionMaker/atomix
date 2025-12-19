using UnityEngine;

public class ElectricStorm : Mob
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
        if (!IsOwner) return;
        PlaySpinAnimation();
        if (Time.time - lastTargetResetTime >= targetResetInterval) ChooseRandomTarget();
        MoveTowardsRandomTarget();
        CheckCollisions();
        TickVelocity();
        UpdateHealthBar();
        DieIfDead();
    }

    void PlaySpinAnimation()
    {
        GFX.Rotate(0, 0, -1000f * Time.deltaTime);
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
}
