using UnityEngine;
using Unity.Netcode;

public class TurnManager : NetworkBehaviour
{
    public enum Turn { Player1, Player2 }
    public NetworkVariable<Turn> currentTurn = new NetworkVariable<Turn>(Turn.Player1);
    public NetworkVariable<int> turnCount = new NetworkVariable<int>(1);

    public delegate void TurnChangeHandler(Turn newTurn, int newTurnCount);
    public event TurnChangeHandler OnTurnChanged;

    private NetworkVariable<bool> player1MadeMove = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> player2MadeMove = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentTurn.Value = Turn.Player1;
            turnCount.Value = 1;
            player1MadeMove.Value = false;
            player2MadeMove.Value = false;
        }

        currentTurn.OnValueChanged += HandleTurnChanged;
    }

    private void HandleTurnChanged(Turn previousTurn, Turn newTurn)
    {
        OnTurnChanged?.Invoke(newTurn, turnCount.Value);
    }

    [ServerRpc(RequireOwnership = false)] 
    public void RequestEndTurnServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        Turn clientTurn = GetPlayerTurn(clientId);

        if (clientTurn == currentTurn.Value)
        {
            if (clientTurn == Turn.Player1) 
            {
                player1MadeMove.Value = true;
            }else if (clientTurn == Turn.Player2) 
            {
                player2MadeMove.Value = true;
            }

            if(player1MadeMove.Value && player2MadeMove.Value) 
            {
                turnCount.Value++;
                player1MadeMove.Value = false;
                player2MadeMove.Value = false;
            }

            currentTurn.Value = (currentTurn.Value == Turn.Player1) ? Turn.Player2 : Turn.Player1;
            OnTurnChanged?.Invoke(currentTurn.Value, turnCount.Value);
            Debug.Log("Turn changed: " + currentTurn.Value + " Turn number: " + turnCount.Value);
        }
        else
        {
            Debug.LogWarning($"Client {clientId} tried to end turn out of sequence");
        }
    }

    private Turn GetPlayerTurn(ulong clientId)
    {
        return clientId == 0 ? Turn.Player1 : Turn.Player2;
    }

    public bool IsCurrentPlayerTurn(ulong clientId) 
    {
        return GetPlayerTurn(clientId) == currentTurn.Value;
    }
}