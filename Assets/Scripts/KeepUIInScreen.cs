using UnityEngine;

public class KeepUIInScreen : MonoBehaviour
{
    RectTransform rect;
    Canvas canvas;
    public float padding;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void LateUpdate()
    {
        KeepInsideScreen();
    }

    void KeepInsideScreen()
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        Vector3 offset = Vector3.zero;

        float left = corners[0].x;
        float right = corners[2].x;
        float bottom = corners[0].y;
        float top = corners[2].y;

        if (left < 0) offset.x += -left + padding;
        if (right > Screen.width) offset.x -= right - Screen.width - padding;
        if (bottom < 0) offset.y += -bottom + padding;
        if (top > Screen.height) offset.y -= top - Screen.height - padding;

        rect.position += offset;
    }
}