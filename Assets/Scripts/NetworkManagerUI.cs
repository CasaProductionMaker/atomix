using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        FindFirstObjectByType<MobSpawner>().startGame();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
