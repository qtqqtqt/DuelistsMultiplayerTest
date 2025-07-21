using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class TurnUI : NetworkBehaviour
{
    public Button endTurnButton;
    public TextMeshProUGUI turnStatusText;
    public TextMeshProUGUI turnCountText;

    private TurnManager turnManager;

    private void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();

        if (turnManager == null)
        {
            Debug.LogError("TurnManager not found");
            return;
        }

        turnManager.currentTurn.OnValueChanged += OnTurnChanged;
        turnManager.turnCount.OnValueChanged += OnTurnCountChanged;

        endTurnButton.onClick.AddListener(OnEndTurnButtonClicked);

        UpdateTurnStatus(turnManager.currentTurn.Value);
        UpdateTurnCount(turnManager.turnCount.Value);
    }

    private void OnEndTurnButtonClicked()
    {
        turnManager.RequestEndTurnServerRpc();
    }

    private void OnTurnChanged(TurnManager.Turn previousTurn, TurnManager.Turn newTurn)
    {
        UpdateTurnStatus(newTurn);
    }

    private void OnTurnCountChanged(int previousCount, int newCount)
    {
        UpdateTurnCount(newCount);
    }

    private void UpdateTurnStatus(TurnManager.Turn currentTurn)
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        if ((currentTurn == TurnManager.Turn.Player1 && localClientId == 0) ||
            (currentTurn == TurnManager.Turn.Player2 && localClientId == 1))
        {
            turnStatusText.text = "Your Turn";
            endTurnButton.interactable = true;
        }
        else
        {
            turnStatusText.text = "Opponent's Turn";
            endTurnButton.interactable = false;
        }
    }

    private void UpdateTurnCount(int newTurnCount)
    {
        turnCountText.text = "Turn: " + newTurnCount;
    }
}
