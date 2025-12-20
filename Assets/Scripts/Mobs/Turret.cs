using NUnit.Framework;
using UnityEngine;

public class Turret : Mob
{
    public Transform turretTop;
    public LineRenderer[] laserShooters;
    public float laserDamage = 15;
    public float laserDistance = 5f;
    float lastHit;

    void Update()
    {
        UpdateHealthBar();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        SpinTurret();
        LaserDamage();
        DieIfDead();
    }

    void SpinTurret()
    {
        turretTop.Rotate(0, 0, -70f * Time.deltaTime);
    }

    void LaserDamage()
    {
        foreach(LineRenderer line in laserShooters)
        {
            Transform laserPoint = line.transform;
            Vector3 hitPosition = laserPoint.position + laserPoint.up * laserDistance;

            RaycastHit2D hit = Physics2D.Raycast(laserPoint.position, laserPoint.up, laserDistance);
            if(hit)
            {
                GameObject hitObject = hit.collider.gameObject;

                if (hitObject.TryGetComponent(out Player player) && Time.time - lastHit > 1f)
                {
                    player.TakeDamage(laserDamage);
                    lastHit = Time.time;
                }
                if (hitObject.TryGetComponent(out Summon summon) && Time.time - lastHit > 1f)
                {
                    summon.TakeDamage(laserDamage);
                    lastHit = Time.time;
                }
                hitPosition = hit.point;
            }

            line.SetPosition(0, laserPoint.position);
            line.SetPosition(1, hitPosition);
        }
    }
}
