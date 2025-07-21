using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public static int[] moveValues = new int[2];

    public float moveSpeed = 5f;
    public Vector2Int gridPosition;
    public SkillShooter spellShooter;
    public int currentMoveValue;

    private GridManager gridManager;
    private TurnManager turnManager;

    private int playerNumber;
    private int gridOffset;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        gridManager = GridManager.Instance;
        spellShooter = GetComponent<SkillShooter>();
        turnManager = FindFirstObjectByType<TurnManager>();

        turnManager.turnCount.OnValueChanged += OnTurnCountChanged;

        InitializePlayer();

        currentMoveValue = turnManager.turnCount.Value;
        UpdateCurrentMoveValue(currentMoveValue, turnManager.turnCount.Value);
    }

    private void InitializePlayer()
    {
        playerNumber = IsHost ? 1 : 2;
        Vector2Int startPosition = new Vector2Int(gridManager.gridSize / 2, gridManager.gridSize / 2);
        gridPosition = startPosition;

        gridOffset = IsHost ? 0 : gridManager.gridSize + 1;
        RequestPositionSyncServerRpc(CalculateWorldPosition(gridPosition));
    }

    private void OnTurnCountChanged(int previousCount, int newCount)
    {
        UpdateCurrentMoveValue(newCount, turnManager.turnCount.Value);
    }


    private void UpdateCurrentMoveValue(int newTurnCount, int maxMoveValue) 
    {
        currentMoveValue = newTurnCount;
        SyncMoveValueToNewClientServerRpc(currentMoveValue, playerNumber, maxMoveValue);
    }

    void Update()
    {
        if (!IsOwner || !turnManager.IsCurrentPlayerTurn(OwnerClientId)) return;

        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDiceServerRpc();
        }
    }

    void HandleMovement()
    {
        if (currentMoveValue <= 0) return;
        if (Input.GetKeyDown(KeyCode.W)) MoveToTile(gridPosition + Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.S)) MoveToTile(gridPosition + Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.A)) MoveToTile(gridPosition + Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.D)) MoveToTile(gridPosition + Vector2Int.right);


    }

    void MoveToTile(Vector2Int newPosition)
    {
        Tile targetTile = gridManager.GetTile(newPosition, playerNumber);
        if (targetTile != null && !targetTile.isOccupied.Value)
        {
            gridManager.GetTile(gridPosition, playerNumber).SetOccupied(false);
            gridPosition = newPosition;
            targetTile.SetOccupied(true);
            currentMoveValue--;
            UpdateMoveValueOnServerServerRpc(currentMoveValue, playerNumber, turnManager.turnCount.Value);
            RequestPositionSyncServerRpc(CalculateWorldPosition(gridPosition));
        }
    }

   

    [ServerRpc]
    void RequestPositionSyncServerRpc(Vector3 position)
    {
        transform.position = position;
        SyncPositionClientRpc(transform.position);
    }

    [ClientRpc]
    void SyncPositionClientRpc(Vector3 position)
    {
        transform.position = position;
    }

    [ServerRpc]
    void RollDiceServerRpc()
    {
        int[] results = DiceRoller.Instance.RollDice(3);
        UpdateDiceResultsClientRpc(results);
    }

    [ClientRpc]
    void UpdateDiceResultsClientRpc(int[] results)
    {
        Debug.Log("Dice Results: " + string.Join(", ", results));
    }

    [ServerRpc]
    void UpdateMoveValueOnServerServerRpc(int moveValue, int player, int maxMovesValue)
    {
        currentMoveValue = moveValue;
        UpdateMoveValueClientRpc(moveValue, player, maxMovesValue);
    }

    [ServerRpc]
    void SyncMoveValueToNewClientServerRpc(int moveValue, int playerNumber, int maxMoveValue)
    {
        moveValues[playerNumber - 1] = moveValue;
        for (int i = 0;i < moveValues.Length; i++) 
        {
            UpdateMoveValueClientRpc(moveValues[i], i + 1, maxMoveValue);
        }
    }

    [ClientRpc]
    void UpdateMoveValueClientRpc(int moveValue, int player, int maxMovesValue)
    {
        FindFirstObjectByType<MultiplayerUI>().UpdateMovementValue(moveValue, player, maxMovesValue);
    }

    private Vector3 CalculateWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * GridManager.Instance.tileSpacing, gridPos.y * GridManager.Instance.tileSpacing + gridOffset, 0);
    }
}