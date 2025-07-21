using UnityEngine;
using Unity.Netcode;

public class GridManager : NetworkBehaviour
{
    public static GridManager Instance;

    public int gridSize = 8;
    public float tileSpacing = 1.1f;
    public Tile[,] player1Grid;
    public Tile[,] player2Grid;
    public GameObject tilePrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnNetworkSpawn()
    {
        SyncGridOnClient();
        if (!IsServer) return;

        player1Grid = new Tile[gridSize, gridSize];
        player2Grid = new Tile[gridSize, gridSize];

        CreateGrid(player1Grid, 1, 0);
        CreateGrid(player2Grid, 2, gridSize + 1);
    }

    private void CreateGrid(Tile[,] grid, int gridOwner, int yOffset)
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 tilePosition = new Vector3(x * tileSpacing, y * tileSpacing + yOffset, 0);
                GameObject tileObj = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tileObj.GetComponent<NetworkObject>().Spawn();

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);

                Tile tile = tileObj.GetComponent<Tile>();

                if (x == gridSize / 2 && y == gridSize / 2) tile.SetOccupied(true);

                tile.name = $"{x},{y}";
                tile.gridPosition.Value = new Vector2Int(x, y);
                tile.gridOwner.Value = gridOwner;

                grid[x, y] = tile;
            }
        }
    }

    public Tile GetTile(Vector2Int position, int gridOwner)
    {
        if (gridOwner == 1)
        {
            if (position.x >= 0 && position.x < gridSize && position.y >= 0 && position.y < gridSize)
            {
                return player1Grid[position.x, position.y];
            }
        }
        else if (gridOwner == 2)
        {
            if (position.x >= 0 && position.x < gridSize && position.y >= 0 && position.y < gridSize)
            {
                return player2Grid[position.x, position.y];
            }
        }
        return null;
    }

    private void SyncGridOnClient()
    {
        if (IsServer) return;

        player1Grid = new Tile[gridSize, gridSize];
        player2Grid = new Tile[gridSize, gridSize];

        foreach (Tile tile in FindObjectsByType<Tile>(FindObjectsSortMode.None))
        {
            if (tile.gridOwner.Value == 1)
                player1Grid[tile.gridPosition.Value.x, tile.gridPosition.Value.y] = tile;
            else if (tile.gridOwner.Value == 2)
                player2Grid[tile.gridPosition.Value.x, tile.gridPosition.Value.y] = tile;
        }

        Debug.Log("Client Grid Synced");
    }
}