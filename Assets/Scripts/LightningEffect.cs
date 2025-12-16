using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    LineRenderer lineRenderer;
    public float duration = 0.1f;

    public void Initialize(Vector2 start, Vector2 end, int quality = 5)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = quality;
        for (int i = 0; i < quality; i++)
        {
            float t = (float)i / (quality - 1);
            Vector2 position = Vector2.Lerp(start, end, t);
            if (i > 0 && i < quality - 1)position += Random.insideUnitCircle * 0.6f;
            lineRenderer.SetPosition(i, position);
        }
        Destroy(gameObject, duration);
    }
}
