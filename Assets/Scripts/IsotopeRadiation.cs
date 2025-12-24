using Unity.Netcode;
using UnityEngine;

public class IsotopeRadiation : NetworkBehaviour
{
    public float size = 6f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color radiationColor = new Color(0f, 1f, 0f, 0.5f);

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = new Color(radiationColor.r, radiationColor.g, radiationColor.b, 0.5f - (transform.localScale.x / (2 * size)));
        if (!IsOwner) return;

        transform.localScale += Vector3.one * 3f * Time.deltaTime;
        transform.localPosition = Vector3.zero;
        if (transform.localScale.x >= size) Destroy(gameObject);
    }
}
