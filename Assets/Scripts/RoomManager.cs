using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

public class RoomManager : NetworkBehaviour
{
    public static RoomManager Instance;

    public NetworkList<RoomData> rooms;

    public RoomData selectedRoomData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        rooms = new NetworkList<RoomData>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void CreateRoomServerRpc(FixedString32Bytes roomName, FixedString32Bytes password, bool isPrivate, ServerRpcParams rpcParams = default)
    {
        RoomData newRoom = new RoomData
        {
            roomName = roomName,
            password = password,
            hostId = rpcParams.Receive.SenderClientId,
            isPrivate = isPrivate
        };
        rooms.Add(newRoom);
        Debug.Log($"Room created: {roomName}");
    }

    [ServerRpc(RequireOwnership = false)]
    public void JoinRoomServerRpc(FixedString32Bytes roomName, FixedString32Bytes password, ServerRpcParams rpcParams = default)
    {
        RoomData? foundRoom = null;
        foreach (RoomData room in rooms)
        {
            if (room.roomName == roomName)
            {
                foundRoom = room;
                break;
            }
        }

        if (foundRoom.HasValue)
        {
            RoomData room = foundRoom.Value;
            if (room.isPrivate && room.password.ToString() != password)
            {
                Debug.Log("Incorrect password");
                return;
            }

            Debug.Log($"Player joined room: {roomName}");
            // Add logic to move the player to the room (e.g., load a new scene)
        }
        else
        {
            Debug.Log("Room not found");
        }
    }



    [ServerRpc(RequireOwnership = false)]
    public void DeleteRoomServerRpc(string roomName)
    {
        RoomData? foundRoom = null;
        foreach (RoomData room in rooms)
        {
            if (room.roomName.ToString() == roomName)
            {
                foundRoom = room;
                break;
            }
        }
        if (foundRoom.HasValue)
        {
            RoomData room = foundRoom.Value;
            rooms.Remove(room);
            Debug.Log($"Room deleted: {roomName}");
        }
    }

}
