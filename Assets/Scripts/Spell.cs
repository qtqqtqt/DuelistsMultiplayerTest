using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
public class Spell : ScriptableObject
{
    public string spellName;
    public int manaCost;
    public int damage;
    public GameObject spellEffectPrefab;
}
