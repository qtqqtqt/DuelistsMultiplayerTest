using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    public GameObject playerPrefab;
    public GameObject gridManagerPrefab;
    public GameObject menuPanel;

    private NetworkVariable<ulong> player1ClientId = new NetworkVariable<ulong>();
    private NetworkVariable<ulong> player2ClientId = new NetworkVariable<ulong>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(menuPanel!= null) 
            {
                menuPanel.SetActive(!menuPanel.activeSelf);
            }
        }
    }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {  
            if (player1ClientId.Value == 0)
            {
                player1ClientId.Value = NetworkManager.Singleton.ConnectedClientsIds[0];
            }
            if (player2ClientId.Value == 0 && NetworkManager.Singleton.ConnectedClientsIds.Count > 1)
            {
                player2ClientId.Value = NetworkManager.Singleton.ConnectedClientsIds[1];
            }
            NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;
        }

    }

    private void HandleSceneEvent(SceneEvent sceneEvent)
    {
        if (sceneEvent.SceneEventType == SceneEventType.LoadComplete &&
            sceneEvent.SceneName == "Game")
        {
            SpawnPlayers();
        }
        NetworkManager.Singleton.SceneManager.OnSceneEvent -= HandleSceneEvent;
    }

    public void SpawnPlayers()
    {
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClientsIds.Count; i++)
        {
            ulong clientId = NetworkManager.Singleton.ConnectedClientsIds[i];
            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
            Debug.Log(i);
        }
    }

    public void ReturnToMainMenu() 
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }

        SceneManager.LoadScene("MainMenu");
    }
}
   

