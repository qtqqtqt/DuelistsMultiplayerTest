using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject loadingScreen;

    public GameObject createRoomPanel;
    public GameObject joinRoomPanel;
    public GameObject passwordPopup;

    public Button createRoomButton;
    public Button joinRoomPopupButton;
    public Button cancelPopupButton;

    public TMP_InputField roomNameInput;
    public TMP_InputField createRoomPasswordInput;
    public TMP_InputField passwordPopupInput;


    public Transform roomListContent;
    public GameObject roomListItemPrefab;

    private void Start()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomButtonClicked);

        joinRoomPopupButton.onClick.AddListener(OnJoinRoomPopupButtonClicked);
        cancelPopupButton.onClick.AddListener(OnCancelPopupButtonClicked);

        passwordPopup.SetActive(false);
    }
    public void OnHostButtonClicked()
    {
        createRoomPanel.SetActive(true);
    }

    public void OnJoinButtonClicked()
    {
        joinRoomPanel.SetActive(true);
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started successfully");
        }
        else
        {
            Debug.LogError("Failed to start client");
        }
        RefreshRoomList();
    }

    private void OnCreateRoomButtonClicked()
    {
        FixedString32Bytes roomName = (FixedString32Bytes)roomNameInput.text;
        FixedString32Bytes password = (FixedString32Bytes)createRoomPasswordInput.text;
        bool isPrivateRoom;
        if (roomName.IsEmpty)
        {
            Debug.Log("Room name cannot be empty");
            return;
        }

        if (password.IsEmpty)
        {
            isPrivateRoom = false;
        }
        else 
        {
            isPrivateRoom = true;
        }

        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started successfully");
            createRoomPanel.SetActive(false);
            RoomManager.Instance.CreateRoomServerRpc(roomName, password, isPrivateRoom);
            loadingScreen.SetActive(true);
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("Failed to start host");
        }
       
      

        
    }

    private void OnJoinRoomPopupButtonClicked()
    {
        FixedString32Bytes password = passwordPopupInput.text;
        JoinRoom(RoomManager.Instance.selectedRoomData.roomName, password);
        passwordPopup.SetActive(false);
    }

    private void JoinRoom(FixedString32Bytes roomName, FixedString32Bytes password)
    {
         joinRoomPanel.SetActive(false);
         loadingScreen.SetActive(true);
         RoomManager.Instance.JoinRoomServerRpc(roomName, password);
    }

    private void RefreshRoomList()
    {
        Debug.Log("Refreshing...");
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (RoomData room in RoomManager.Instance.rooms)
        {
            Debug.Log(room.roomName);
            GameObject roomListItem = Instantiate(roomListItemPrefab, roomListContent);
            roomListItem.GetComponent<RoomListItem>().Initialize(room, passwordPopup, passwordPopupInput);
        }
    }

    private void OnCancelPopupButtonClicked()
    {
        passwordPopup.SetActive(false);
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}