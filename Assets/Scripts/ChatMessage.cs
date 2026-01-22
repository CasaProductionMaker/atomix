using TMPro;
using UnityEngine;

public class ChatMessage : MonoBehaviour
{
    public TextMeshProUGUI messageText;

    public void SetText(string text)
    {
        messageText.text = text;
    }
}