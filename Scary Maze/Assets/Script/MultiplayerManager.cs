using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Netcode;

public class MultiplayerManager : MonoBehaviour
{
    public TMP_InputField joinCodeInput;
    public TMP_Text generatedCodeText;
    public GameObject startGameButton;

    private string joinCode;
    private bool isHost = false;

    private async void Awake()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        startGameButton.SetActive(false);
        generatedCodeText.text = "";
    }

    public async void CreateLobby()
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(2);
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        generatedCodeText.text = "Código da Sala: " + joinCode;
        isHost = true;

        // Guarda os dados para carregar na próxima cena depois
        PlayerPrefs.SetString("JOINCODE", joinCode);
        PlayerPrefs.SetInt("ISHOST", 1);

        startGameButton.SetActive(true);
    }

    public void JoinLobby()
    {
        joinCode = joinCodeInput.text;
        if (string.IsNullOrEmpty(joinCode)) return;

        PlayerPrefs.SetString("JOINCODE", joinCode);
        PlayerPrefs.SetInt("ISHOST", 0);

        SceneManager.LoadScene("MazeScene");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MazeScene");
    }
}
