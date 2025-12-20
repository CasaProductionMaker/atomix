using UnityEngine;

public class Stunner : Mob
{
    public SingleShooter[] shooters;
    public float shootInterval = 1f;
    public GameObject bulletPrefab;
    float lastShot = -10f;

    void Update()
    {
        UpdateHealthBar();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        ShooterLogic();
        DieIfDead();
    }

    void ShooterLogic()
    {
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
                    Debug.Log(angleToPlayer);
                    float shooterRotation = shooter.defaultAngle + GFX.localEulerAngles.z;
                    shooterRotation = shooterRotation % 360;
                    Debug.Log(shooterRotation);
                    if (
                        (angleToPlayer < shooterRotation + 90 && angleToPlayer > shooterRotation - 90) ||
                        (angleToPlayer + 360 < shooterRotation + 90 && angleToPlayer + 360 > shooterRotation - 90) ||
                        (angleToPlayer - 360 < shooterRotation + 90 && angleToPlayer - 360 > shooterRotation - 90)
                    ) {
                        shooter.transform.rotation = Quaternion.AngleAxis(angleToPlayer - 90, Vector3.forward);
                        if (Time.time - lastShot >= shootInterval)
                        {
                            GameObject bullet = Instantiate(bulletPrefab, shooter.shootPoint.position, shooter.shootPoint.rotation);
                            bullet.GetComponent<StunBullet>().fixedVelocity = shooter.shootPoint.right;
                            lastShot = Time.time;
                        }
                    }
                }
            }
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
