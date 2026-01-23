using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Singleton;
    [SerializeField] GameObject chatMessagePrefab;
    [SerializeField] Transform chatContainer;
    [SerializeField] ScrollRect chatScrollView;
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] TMP_InputField usernameInput;
    public InputActionReference sendInput;
    public string username;
    public bool isOp = false;
    
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
        AttemptExecuteCommand(message);
    }

    public void AttemptExecuteCommand(string message)
    {
        if (!message.StartsWith("/")) { return; }
        if (!isOp) { return; }
        string commandName = message.Split(" ")[0].Substring(1).ToLower();
        List<string> args = new List<string>(message.Split(" "));
        args.RemoveAt(0);
        switch (commandName)
        {
            case "tp":
                if (args.Count != 3)
                {
                    SendChatMessage("Usage: /tp username x y", "Server");
                    return;
                }

                string targetUsername = args[0];
                float x, y;
                if (!float.TryParse(args[1], out x) || !float.TryParse(args[2], out y))
                {
                    SendChatMessage("Invalid coordinates. Usage: /tp username x y", "Server");
                    return;
                }

                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject player in players)
                {
                    Player foundPlayerScript = player.GetComponent<Player>();
                    if (foundPlayerScript != null && foundPlayerScript.playerUsername.Value.ToString() == targetUsername)
                    {
                        foundPlayerScript.teleportPlayerOwnerRpc(new Vector2(x, y));
                        SendChatMessage("Teleported " + targetUsername + " to (" + x + ", " + y + ")", "Server");
                        return;
                    }
                }

                SendChatMessage("Player not found: " + targetUsername, "Server");
                break;
            case "add_electron":
                if (args.Count != 2)
                {
                    SendChatMessage("Usage: /add_electron username electron_id", "Server");
                    return;
                }

                string targetUser = args[0];
                int electron_id;
                if (!int.TryParse(args[1], out electron_id))
                {
                    SendChatMessage("Invalid amount. Usage: /add_electron username electron_id", "Server");
                    return;
                }

                GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject player in allPlayers)
                {
                    Player foundPlayer = player.GetComponent<Player>();
                    PlayerElectronController foundPlayerEC = player.GetComponent<PlayerElectronController>();
                    if (foundPlayer != null && foundPlayer.playerUsername.Value.ToString() == targetUser)
                    {
                        LangObject langObject = GameObject.Find("LangObject").GetComponent<LangObject>();
                        GameObject electronPrefab = langObject.electronsInGame[electron_id];
                        if (electronPrefab == null)
                        {
                            SendChatMessage("Invalid electron ID: " + electron_id, "Server");
                            return;
                        }

                        foundPlayerEC.AddElectronCommandOwnerRpc(electron_id);
                        SendChatMessage("Added electron #" + electron_id + " to " + targetUser, "Server");
                        return;
                    }
                }

                SendChatMessage("Player not found: " + targetUser, "Server");
                break;
            case "kill":
                if (args.Count != 1)
                {
                    SendChatMessage("Usage: /kill username", "Server");
                    return;
                }

                string killTargetUser = args[0];

                GameObject[] killPlayers = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject player in killPlayers)
                {
                    Player foundPlayerScript = player.GetComponent<Player>();
                    if (foundPlayerScript != null && foundPlayerScript.playerUsername.Value.ToString() == killTargetUser)
                    {
                        foundPlayerScript.TakeDamageOwnerRpc(100000f);
                        SendChatMessage("Killed " + killTargetUser, "Server");
                        return;
                    }
                }

                SendChatMessage("Player not found: " + killTargetUser, "Server");
                break;
            case "spawn_mob":
                if (args.Count != 3)
                {
                    SendChatMessage("Usage: /spawn_mob mob_id x y", "Server");
                    return;
                }

                int mob_id;
                float mob_x, mob_y;
                if (!int.TryParse(args[0], out mob_id) || !float.TryParse(args[1], out mob_x) || !float.TryParse(args[2], out mob_y))
                {
                    SendChatMessage("Invalid arguments. Usage: /spawn_mob mob_id x y", "Server");
                    return;
                }

                MobSpawner mobSpawner = FindFirstObjectByType<MobSpawner>();
                if (mobSpawner == null)
                {
                    return;
                }

                mobSpawner.SpawnMobAtPositionServerRpc(mob_id, new Vector2(mob_x, mob_y));
                SendChatMessage("Spawned mob #" + mob_id + " at (" + mob_x + ", " + mob_y + ")", "Server");
                break;
            case "set_map_size":
                if (args.Count != 2)
                {
                    SendChatMessage("Usage: /set_map_size width height", "Server");
                    return;
                }

                float width, height;
                if (!float.TryParse(args[0], out width) || !float.TryParse(args[1], out height))
                {
                    SendChatMessage("Invalid dimensions. Usage: /set_map_size width height", "Server");
                    return;
                }

                MobSpawner spawner = FindFirstObjectByType<MobSpawner>();
                if (spawner == null)
                {
                    return;
                }

                spawner.SetMapSizeServerRpc(new Vector2(width, height));
                SendChatMessage("Set map size to (" + width + ", " + height + ")", "Server");
                break;
            case "set_wave":
                if (args.Count != 1)
                {
                    SendChatMessage("Usage: /set_wave wave_number", "Server");
                    return;
                }

                int waveNumber;
                if (!int.TryParse(args[0], out waveNumber))
                {
                    SendChatMessage("Invalid wave number. Usage: /set_wave wave_number", "Server");
                    return;
                }

                MobSpawner waveSpawner = FindFirstObjectByType<MobSpawner>();
                if (waveSpawner == null)
                {
                    return;
                }

                waveSpawner.SetWaveNumberServerRpc(waveNumber);
                SendChatMessage("Set wave number to " + waveNumber, "Server");
                break;
            case "op":
                if(args.Count != 1)
                {
                    SendChatMessage("Usage: /op username", "Server");
                    return;
                }

                string opTargetUser = args[0];
                GameObject[] opPlayers = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject player in opPlayers)
                {
                    Player foundPlayerScript = player.GetComponent<Player>();
                    if (foundPlayerScript != null && foundPlayerScript.playerUsername.Value.ToString() == opTargetUser)
                    {
                        ChatManager chatManager = foundPlayerScript.GetComponent<ChatManager>();
                        if (chatManager != null)
                        {
                            chatManager.SetOpOwnerRpc(true);
                            SendChatMessage("Granted operator status to " + opTargetUser, "Server");
                            return;
                        }
                    }
                }

                SendChatMessage("Player not found: " + opTargetUser, "Server");
                break;
            case "deop":
                if(args.Count != 1)
                {
                    SendChatMessage("Usage: /deop username", "Server");
                    return;
                }

                string deopTargetUser = args[0];
                GameObject[] deopPlayers = GameObject.FindGameObjectsWithTag("Player");
                foreach(GameObject player in deopPlayers)
                {
                    Player foundPlayerScript = player.GetComponent<Player>();
                    if (foundPlayerScript != null && foundPlayerScript.playerUsername.Value.ToString() == deopTargetUser)
                    {
                        ChatManager chatManager = foundPlayerScript.GetComponent<ChatManager>();
                        if (chatManager != null)
                        {
                            chatManager.SetOpOwnerRpc(true);
                            SendChatMessage("Granted operator status to " + deopTargetUser, "Server");
                            return;
                        }
                    }
                }

                SendChatMessage("Player not found: " + deopTargetUser, "Server");
                break;
            default:
                AddMessage("Unknown command: " + commandName);
                break;
        }
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

    [Rpc(SendTo.Owner, InvokePermission = RpcInvokePermission.Everyone)]
    public void SetOpOwnerRpc(bool isOp)
    {
        this.isOp = isOp;
    }
    
    void AddMessage(string str)
    {
        GameObject newMessage = Instantiate(chatMessagePrefab, chatContainer);
        newMessage.GetComponent<ChatMessage>().SetText(str);
        Canvas.ForceUpdateCanvases();
        chatScrollView.verticalNormalizedPosition = 0f;
    }
}
