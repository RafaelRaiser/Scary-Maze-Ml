using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class MultiplayerManager : MonoBehaviour
{
    public TMP_InputField codeInput;
    public GameObject lobbyPanel;

    private async void Start()
    {
        await Unity.Services.Core.UnityServices.InitializeAsync();
    }

    public async void CreateLobby()
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(2);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        NetworkManager.Singleton.StartHost();
        codeInput.text = joinCode;
        lobbyPanel.SetActive(false);
    }

    public async void JoinLobby()
    {
        string joinCode = codeInput.text;

        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
        lobbyPanel.SetActive(false);
    }
}
