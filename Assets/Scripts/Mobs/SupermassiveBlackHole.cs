using UnityEngine;

public class SupermassiveBlackHole : Mob
{
    public float radius = 5f;
    public float pull = 0.3f;
    public Transform disc;

    void Update()
    {
        UpdateHealthBar();
        if (!IsOwner) return;
        TickVelocity();
        DiscSpinAnimation();
        PullPlayers();
        DieIfDead();
    }

    void DiscSpinAnimation()
    {
        disc.Rotate(0f, 0f, 250f * Time.deltaTime);
    }

    void PullPlayers()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out Player player))
            {
                Vector2 pullDirection = transform.position - player.transform.position;
                float pullAmount = pull / pullDirection.magnitude;
                pullDirection = pullDirection.normalized * pullAmount;
                player.ApplyVelocity(pullDirection);
            }

            if (collider.gameObject.TryGetComponent(out Electron electron))
            {
                Vector3 pullDirection = transform.position - electron.transform.position;
                if (pullDirection.magnitude == 0) continue;
                float pullAmount = pull * 0.1f / pullDirection.magnitude;
                pullDirection = pullDirection.normalized * pullAmount;
                electron.transform.position += pullDirection;
            }
        }
    }
}
