using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;
    [SerializeField] GameObject chatMessagePrefab;
    [SerializeField] Transform chatContainer;
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] TMP_InputField usernameInput;
    public InputActionReference sendInput;
    public string username;
    
    void Awake()
    {
        ChatManager.Singleton = this;
        sendInput.action.performed += ctx =>
        {
            if (string.IsNullOrWhiteSpace(chatInput.text)) {
                chatInput.Select();
            } else {
                SendChatMessage(chatInput.text, username);
                chatInput.text = "";
            }
        };
    }

    public bool isChatSelected()
    {
        return chatInput.isFocused;
    }

    public void UpdateUsername()
    {
        username = usernameInput.text;
    }

    public void SendChatMessage(string message, string sender)
    {
        SendMessageServerRpc(sender + ": " + message);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SendMessageServerRpc(string message)
    {
        RevieveChatMessageClientRpc(message);
    }

    [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Everyone)]
    public void RevieveChatMessageClientRpc(string message)
    {
        AddMessage(message);
    }
    
    void AddMessage(string str)
    {
        GameObject newMessage = Instantiate(chatMessagePrefab, chatContainer);
        newMessage.GetComponent<ChatMessage>().SetText(str);
    }
}
