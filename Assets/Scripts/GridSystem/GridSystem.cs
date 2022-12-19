using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem
{
    private int width;
    private int height;
    private float cellSize;
    private GridObject[,] gridObjects;

    public GridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjects = new GridObject[width, height];
        for (int x = 0; x < this.width; x++)
        {
            for (int z = 0; z < this.height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjects[x, z] = new GridObject(this, gridPosition);
            }
        }

    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(Mathf.RoundToInt(worldPosition.x / cellSize), Mathf.RoundToInt(worldPosition.z / cellSize));
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, new Vector3(GetWorldPosition(gridPosition).x, 0.2f, GetWorldPosition(gridPosition).z), Quaternion.identity);
                GridDebugObject debugObject = debugTransform.GetComponent<GridDebugObject>();
                debugObject.SetGridObject(gridObjects[x, z]);
            }
        }
    }

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjects[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x < width && gridPosition.z < height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}
