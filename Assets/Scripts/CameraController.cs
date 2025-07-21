using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GridManager gridManager;

    void Start()
    {
        if (gridManager == null) gridManager = GridManager.Instance;

        float gridWidth = gridManager.gridSize * gridManager.tileSpacing * 2;
        float gridHeight = gridManager.gridSize * gridManager.tileSpacing + (gridManager.gridSize + 1) * gridManager.tileSpacing;

        transform.position = new Vector3(0, gridHeight / 2 - 0.5f, -10);

        Camera camera = GetComponent<Camera>();
        if (camera.orthographic)
        {
            camera.orthographicSize = Mathf.Max(gridWidth, gridHeight) / 2;
        }
        else
        {
            camera.fieldOfView = 60;
        }
    }
}