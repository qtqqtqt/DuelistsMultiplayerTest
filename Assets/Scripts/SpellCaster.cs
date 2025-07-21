using UnityEngine;

using Unity.Netcode;

public class SpellCaster : NetworkBehaviour
{
    public ManaManager manaManager;
    public Spell[] spells;

    public void CastSpell(int spellIndex)
    {
        if (!IsOwner) return;
        CastSpellServerRpc(spellIndex);
    }

    [ServerRpc]
    void CastSpellServerRpc(int spellIndex)
    {
        if (manaManager.SpendMana(spells[spellIndex].manaCost))
        {
            SpawnSpellEffectClientRpc(spellIndex, transform.position);
        }
    }

    [ClientRpc]
    void SpawnSpellEffectClientRpc(int spellIndex, Vector3 position)
    {
        Instantiate(spells[spellIndex].spellEffectPrefab, position, Quaternion.identity);
    }
}
