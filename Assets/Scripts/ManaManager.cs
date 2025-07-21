using UnityEngine;

using Unity.Netcode;

public class ManaManager : NetworkBehaviour
{
    public NetworkVariable<int> currentMana = new NetworkVariable<int>(3);
    public int maxMana = 10;

    public void GainMana(int amount)
    {
        if (!IsServer) return;
        currentMana.Value = Mathf.Min(currentMana.Value + amount, maxMana);
    }

    public bool SpendMana(int amount)
    {
        if (!IsServer) return false;
        if (currentMana.Value >= amount)
        {
            currentMana.Value -= amount;
            return true;
        }
        return false;
    }
}
