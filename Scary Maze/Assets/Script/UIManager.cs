using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;

public class UIManager : MonoBehaviour
{
    public GameObject victoryPanel;
    public GameObject defeatPanel;

    private async void Awake()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);

        string joinCode = PlayerPrefs.GetString("JOINCODE");
        bool isHost = PlayerPrefs.GetInt("ISHOST") == 1;

        if (isHost)
            await StartHostRelay();
        else
            await StartClientRelay(joinCode);
    }

    private async Task StartHostRelay()
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(2);
        string realJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log("Sala criada, código: " + realJoinCode);

        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }

    private async Task StartClientRelay(string joinCode)
    {
        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }
}
