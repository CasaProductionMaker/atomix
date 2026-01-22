using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public TMP_InputField joinInput;
    public TextMeshProUGUI joinCodeText;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in anonymously: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4, 
                (ushort)allocation.RelayServer.Port, 
                allocation.AllocationIdBytes, 
                allocation.Key, 
                allocation.ConnectionData, 
                allocation.ConnectionData, 
                true
            );
            joinCodeText.text = "Start " + joinCode;

            NetworkManager.Singleton.StartHost();

            ChatManager.Singleton.username = ChatManager.Singleton.username == "" ? GetRandomUsername() : ChatManager.Singleton.username;
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }

    public async void JoinRelay()
    {
        string joinCode = joinInput.text;
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4, 
                (ushort)joinAllocation.RelayServer.Port, 
                joinAllocation.AllocationIdBytes, 
                joinAllocation.Key, 
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData, 
                true
            );

            NetworkManager.Singleton.StartClient();

            ChatManager.Singleton.username = ChatManager.Singleton.username == "" ? GetRandomUsername() : ChatManager.Singleton.username;
        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
    }

    public void StartGame()
    {
        FindFirstObjectByType<MobSpawner>().startGame();
    }

    string GetRandomUsername()
    {
        string[] prefixes = {"Cool", "Nice", "Great", "Amazing", "Godly"};
        string[] names = {"Dog", "Cat", "Snail", "Slug", "Monkey", "Ladybug"};

        int rand1 = Random.Range(0, prefixes.Length);
        int rand2 = Random.Range(0, names.Length);
        
        return prefixes[rand1] + names[rand2];
    }
}
