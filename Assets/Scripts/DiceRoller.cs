using Unity.Netcode;
using UnityEngine;

public class DiceRoller : NetworkBehaviour
{
    public static DiceRoller Instance;
    public Dice dice;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public int[] RollDice(int numDice)
    {
        int[] results = new int[numDice];
        for (int i = 0; i < numDice; i++)
        {
            results[i] = dice.Roll();
        }
        return results;
    }
}