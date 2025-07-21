using UnityEngine;

public class Dice : MonoBehaviour
{
    public int[] faces;
    private int currentFace;

    public int Roll()
    {
        currentFace = faces[Random.Range(0, faces.Length)];
        return currentFace;
    }

    public int GetCurrentFace() => currentFace;
}
