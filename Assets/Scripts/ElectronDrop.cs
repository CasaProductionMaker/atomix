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
        pos.x = Mathf.Clamp(pos.x, -10f, 10f);
        pos.y = Mathf.Clamp(pos.y, -10f, 10f);
        transform.position = pos;
    }
}
