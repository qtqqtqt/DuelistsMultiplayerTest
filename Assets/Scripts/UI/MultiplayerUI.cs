using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class MultiplayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1MoveValue;
    [SerializeField] private TextMeshProUGUI player2MoveValue;

    public void UpdateMovementValue( int moveValue, int player, int maxMovesValue) 
    {
        if (player == 1) 
        {
            player1MoveValue.text = moveValue.ToString() + "/" + maxMovesValue;
        }else if (player == 2) 
        {
            player2MoveValue.text = moveValue.ToString() + "/" + maxMovesValue;
        }
    }
}