using UnityEngine;

using Unity.Netcode;

public class Tile : NetworkBehaviour
{
    public NetworkVariable<Vector2Int> gridPosition = new NetworkVariable<Vector2Int>();
    public NetworkVariable<int> gridOwner = new NetworkVariable<int>();
    public NetworkVariable<bool> isOccupied = new NetworkVariable<bool>(false);
    public NetworkVariable<int> tileEffect = new NetworkVariable<int>(0);

    [SerializeField] private GameObject highlightObject;

    public void SetEffect(int effect)
    {
        if (!IsServer) return;
        tileEffect.Value = effect;
    }

    public void SetOccupied(bool occupied)
    {
        if (IsServer) 
        {
            isOccupied.Value = occupied;
        }
        else
        {
            RequestSetOccupiedServerRpc(occupied);
        }
    }

    private void OnMouseEnter()
    {
        highlightObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlightObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (IsOwner) // Only the owner of the tile can trigger the click
        {
            Debug.Log($"Tile clicked at {gridPosition.Value} on Player {gridOwner.Value}'s grid!");
            RequestCastSpellServerRpc(gridPosition.Value);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSetOccupiedServerRpc(bool occupied)
    {
        isOccupied.Value = occupied;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestCastSpellServerRpc(Vector2Int position)
    {
        // Forward the spell cast request to the SkillShooter
        SkillShooter shooter = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<SkillShooter>();
        if (shooter != null)
        {
           // shooter.ShootSkillServerRpc(position);
        }
    }
}
