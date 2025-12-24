using UnityEngine;

public class BlackHole : Mob
{
    public float radius = 5f;
    public float pull = 0.3f;
    public Transform disc;
    public GameObject supermassiveBlackHole;
    public GameObject mergeEffect;

    void Update()
    {
        UpdateHealthBar();
        RotateGFX();
        DiscSpinAnimation();
        if (!IsOwner) return;
        CheckCollisions();
        TickVelocity();
        PullPlayers();
        DieIfDead();
    }

    void DiscSpinAnimation()
    {
        disc.Rotate(0f, 0f, 200f * Time.deltaTime);
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

            if (collider.gameObject.TryGetComponent(out BlackHole hole) && hole != this)
            {
                Vector3 pullDirection = transform.position - hole.transform.position;
                float pullAmount = pull * 0.1f / pullDirection.magnitude;
                pullDirection = pullDirection.normalized * pullAmount;
                hole.transform.position += pullDirection;
            }
        }
    }

    new void CheckCollisions()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);
        foreach (Collider2D collider in hit)
        {
            if (collider.gameObject.TryGetComponent(out BlackHole hole) && hole != this)
            {
                Vector3 inBetweenPosition = (transform.position + hole.transform.position) / 2;
                Instantiate(supermassiveBlackHole, inBetweenPosition, Quaternion.identity);
                Instantiate(mergeEffect, inBetweenPosition, Quaternion.identity);

                FindFirstObjectByType<MobSpawner>().mobsLeftInWave--;

                Destroy(hole.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
