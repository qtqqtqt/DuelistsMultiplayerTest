using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public TMP_Text roomNameText;
    public Button joinButton;
    public GameObject highlightObject;

    private RoomData roomData;
    private GameObject passwordPopup;
    private TMP_InputField passwordInputField;

    public void Initialize(RoomData data, GameObject popup, TMP_InputField inputField)
    {
        roomData = data;
        roomNameText.text = data.roomName.ToString();

        passwordPopup = popup;
        passwordInputField = inputField;

        joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    private void OnJoinButtonClicked()
    {
        if (roomData.isPrivate)
        {
            // Show the password popup
            passwordPopup.SetActive(true);
            passwordInputField.text = ""; // Clear the input field
        }
        else
        {
            // Join the room directly if it's not private
            // MainMenu.Instance.JoinRoom(roomData.roomName, "");
            Debug.Log("join clicked");
        }
        //RoomManager.Instance.JoinRoomServerRpc(roomData.roomName.ToString(), roomData.password.ToString());
      
    }

    private void OnMouseDown()
    {
        highlightObject.SetActive(true);
        RoomManager.Instance.selectedRoomData= roomData;
    }
}
