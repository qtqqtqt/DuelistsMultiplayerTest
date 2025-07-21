using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    public GameObject loadingScreen;
    public Button readyButton;
    public Button startGameButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI joinedPlayersText;

    private NetworkVariable<bool> isPlayer2Ready = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isGameStarted = new NetworkVariable<bool>(false);

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        readyButton.onClick.AddListener(OnReadyButtonClicked);
        startGameButton.onClick.AddListener(OnStartGameButtonClicked);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        UpdateLobbyUI();
    }

    private void OnClientConnected(ulong clientId)
    {
        UpdateLobbyUI();
        joinedPlayersText.text += "\nPlayer 2";
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            isPlayer2Ready.Value = false;
            isGameStarted.Value = false;
            joinedPlayersText.text = "Player 1";
        }

        isPlayer2Ready.OnValueChanged += OnPlayer2ReadyChanged;
    }

    private void OnReadyButtonClicked()
    {
        if (!IsServer && NetworkManager.Singleton.LocalClientId != 0)
        {
            SetPlayer2ReadyServerRpc(true);
        }
    }

    private void OnStartGameButtonClicked()
    {
        if (IsServer && isPlayer2Ready.Value)
        {
            StartGameServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayer2ReadyServerRpc(bool ready)
    {
        isPlayer2Ready.Value = ready;
    }

    private void OnPlayer2ReadyChanged(bool previous, bool current)
    {
        UpdateLobbyUI();
    }

    private void UpdateLobbyUI()
    {
        if (IsServer)
        {
            readyButton.gameObject.SetActive(false);
            startGameButton.gameObject.SetActive(isPlayer2Ready.Value);
            statusText.text = isPlayer2Ready.Value ? "Player 2 is ready" : "Waiting ...";
        }
        else
        {
            readyButton.gameObject.SetActive(!isPlayer2Ready.Value);
            startGameButton.gameObject.SetActive(false);
            joinedPlayersText.text = "Player 1\nPlayer 2";
            statusText.text = isPlayer2Ready.Value ? "Waiting for Host to start the game..." : "Press Ready";
        }
    }

    [ServerRpc]
    private void StartGameServerRpc()
    {
        isGameStarted.Value = true;
        loadingScreen.SetActive(true);
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode)
    {
        loadingScreen.SetActive(false);
    }
}