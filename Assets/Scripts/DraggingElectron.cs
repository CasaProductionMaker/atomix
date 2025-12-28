using UnityEngine;

public class DraggingElectron : UISlot
{
    [SerializeField] RectTransform rectTransform;
    new void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        base.Start();
    }
}