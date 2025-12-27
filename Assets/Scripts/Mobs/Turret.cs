using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class Turret : Mob
{
    public Transform turretTop;
    public LineRenderer[] laserShooters;
    public NetworkVariable<float> turretTopRotation = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float laserDamage = 15;
    public float laserDistance = 5f;
    float lastHit;

    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        SpinTurret();
        LaserDamage();
        DieIfDead();
    }

    void SpinTurret()
    {
        turretTopRotation.Value -= 70f * Time.deltaTime;
    }

    new void RotateGFX()
    {
        base.RotateGFX();
        turretTop.rotation = Quaternion.Euler(0, 0, turretTopRotation.Value);
        foreach(LineRenderer line in laserShooters)
        {
            Transform laserPoint = line.transform;
            Vector3 hitPosition = laserPoint.position + laserPoint.up * laserDistance;

            RaycastHit2D hit = Physics2D.Raycast(laserPoint.position, laserPoint.up, laserDistance);
            if(hit)
            {
                hitPosition = hit.point;
            }

            line.SetPosition(0, laserPoint.position);
            line.SetPosition(1, hitPosition);
        }
    }

    void LaserDamage()
    {
        foreach(LineRenderer line in laserShooters)
        {
            Transform laserPoint = line.transform;

            RaycastHit2D hit = Physics2D.Raycast(laserPoint.position, laserPoint.up, laserDistance);
            if(hit)
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.TryGetComponent(out Player player) && Time.time - lastHit > 1f)
                {
                    player.TakeDamageOwnerRpc(laserDamage);
                    lastHit = Time.time;
                }
                if (hitObject.TryGetComponent(out Summon summon) && Time.time - lastHit > 1f)
                {
                    summon.TakeDamage(laserDamage);
                    lastHit = Time.time;
                }
            }
        }
    }
}
