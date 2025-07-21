using Unity.Netcode;
using UnityEngine;

public class SkillShooter : NetworkBehaviour
{
    public GridManager gridManager;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        gridManager = GridManager.Instance;
    }

    [ServerRpc]
    public void ShootSkillServerRpc(Vector2Int targetPosition)
    {
        int opponentGridOwner = IsOwner ? 2 : 1;
        Tile targetTile = gridManager.GetTile(targetPosition, opponentGridOwner);

        if (targetTile != null)
        {
            targetTile.SetEffect(1); 
            ApplySkillEffectClientRpc(targetPosition, opponentGridOwner);
        }
    }

    [ClientRpc]
    void ApplySkillEffectClientRpc(Vector2Int targetPosition, int gridOwner)
    {
        Debug.Log($"Skill hit at {targetPosition} on Player {gridOwner}'s grid!");
    }
}