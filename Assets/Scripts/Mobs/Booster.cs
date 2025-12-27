using Unity.Netcode;
using UnityEngine;

public class Booster : Mob
{
    public GameObject boosterEffect;
    public NetworkVariable<bool> isFireOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        ShowEffects();
        if (!IsOwner) return;
        isFireOn.Value = target != null;
        DetectTarget();
        MoveTowardsTarget();
        LookAtTarget();
        CheckCollisions();
        TickVelocity();
        DieIfDead();
    }

    void LookAtTarget()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GFXRotation.Value = angle;
    }

    void ShowEffects()
    {
        boosterEffect.SetActive(isFireOn.Value);
    }
}