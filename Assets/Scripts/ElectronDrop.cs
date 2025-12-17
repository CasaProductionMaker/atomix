using UnityEngine;

public class ElectronDrop : MonoBehaviour
{
    public GameObject electronPrefab;
    public Vector2 spreadDirection;
    public SpriteRenderer electronImage;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = spreadDirection * 2f;
        electronImage.sprite = electronPrefab.GetComponent<SpriteRenderer>().sprite;
    }

    void Update()
    {
        StayInBounds();
    }

    public void StayInBounds()
    {
        Vector3 pos = transform.position;
        Vector2 bounds = FindFirstObjectByType<MobSpawner>().mapSize;
        pos.x = Mathf.Clamp(pos.x, -bounds.x, bounds.x);
        pos.y = Mathf.Clamp(pos.y, -bounds.y, bounds.y);
        transform.position = pos;
    }
}
