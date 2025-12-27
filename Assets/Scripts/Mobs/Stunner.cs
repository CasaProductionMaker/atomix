using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class Stunner : Mob
{
    public SingleShooter[] shooters;
    public float shootInterval = 1f;
    public GameObject bulletPrefab;
    float lastShot = -10f;
    public NetworkVariable<float> shooterAngle1 = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> shooterAngle2 = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    new void Start()
    {
        base.Start();
        shooterAngle1.Value = 0f;
        shooterAngle2.Value = 0f;
    }

    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        UpdateShooterGFX();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        DetectTargets();
        UpdateShooterAngles();
        ShootIfPossible();
        DieIfDead();
    }

    void DetectTargets()
    {
        if (target != null) return;

        int i = 0;
        foreach (SingleShooter shooter in shooters)
        {
            Collider2D[] hit = Physics2D.OverlapCircleAll(shooter.transform.position, aggroRange);
            foreach (Collider2D collider in hit)
            {
                if (collider.gameObject.TryGetComponent(out Player player))
                {
                    Vector3 direction = player.transform.position - shooter.transform.position;
                    float angleToPlayer = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    angleToPlayer -= 90;

                    float shooterRotation = shooter.defaultAngle + GFX.localEulerAngles.z;
                    shooterRotation = shooterRotation % 360;

                    if (
                        (angleToPlayer < shooterRotation + 90 && angleToPlayer > shooterRotation - 90) ||
                        (angleToPlayer + 360 < shooterRotation + 90 && angleToPlayer + 360 > shooterRotation - 90) ||
                        (angleToPlayer - 360 < shooterRotation + 90 && angleToPlayer - 360 > shooterRotation - 90)
                    ) {
                        target = player.transform;
                    }
                }
            }
            i++;
        }
    }

    void UpdateShooterAngles()
    {
        if (target == null) return;

        int i = 0;
        foreach (SingleShooter shooter in shooters)
        {
            Vector3 direction = target.position - shooter.transform.position;
            float angleToPlayer = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angleToPlayer -= 90;

            float shooterRotation = shooter.defaultAngle + GFX.localEulerAngles.z;
            shooterRotation = shooterRotation % 360;

            if (
                (angleToPlayer < shooterRotation + 90 && angleToPlayer > shooterRotation - 90) ||
                (angleToPlayer + 360 < shooterRotation + 90 && angleToPlayer + 360 > shooterRotation - 90) ||
                (angleToPlayer - 360 < shooterRotation + 90 && angleToPlayer - 360 > shooterRotation - 90)
            ) {
                if(i == 0) shooterAngle1.Value = angleToPlayer - 90;
                if(i == 1) shooterAngle2.Value = angleToPlayer - 90;
            }
            i++;
        }
    }

    void UpdateShooterGFX()
    {
        int i = 0;
        foreach (SingleShooter shooter in shooters)
        {
            // ADD LOGS TO SEE LENGTH AND I
            float rotation = 0f;
            if(i == 0) rotation = shooterAngle1.Value;
            if(i == 1) rotation = shooterAngle2.Value;
            shooter.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
            i++;
        }
    }

    void ShootIfPossible()
    {
        if (target == null) return;

        int i = 0;
        foreach (SingleShooter shooter in shooters)
        {
            Vector3 direction = target.position - shooter.transform.position;
            float angleToPlayer = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angleToPlayer -= 90;
            float shooterRotation = shooter.defaultAngle + GFX.localEulerAngles.z;
            shooterRotation = shooterRotation % 360;
            if (
                (angleToPlayer < shooterRotation + 90 && angleToPlayer > shooterRotation - 90) ||
                (angleToPlayer + 360 < shooterRotation + 90 && angleToPlayer + 360 > shooterRotation - 90) ||
                (angleToPlayer - 360 < shooterRotation + 90 && angleToPlayer - 360 > shooterRotation - 90)
            ) {
                if (Time.time - lastShot >= shootInterval)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shooter.shootPoint.position, shooter.shootPoint.rotation);
                    bullet.GetComponent<NetworkObject>().Spawn(true);
                    bullet.GetComponent<StunBullet>().fixedVelocity = shooter.shootPoint.right;
                    lastShot = Time.time;
                }
            }
            i++;
        }
    }
}

[System.Serializable]
public struct SingleShooter
{
    public Transform transform;
    public float defaultAngle;
    public Transform shootPoint;
}
