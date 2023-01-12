using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridSystemVisual;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance;

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterials;

    private GridSystemVisualSingle[,] gridSystemVisualSingles;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        gridSystemVisualSingles = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];

        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingle = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                gridSystemVisualSingles[x, z] = gridSystemVisualSingle.GetComponent<GridSystemVisualSingle>();
            }
        }

        HideAllGridPositions();
        UnitActionSystem.Instance.OnSelectedActionChanged += Instance_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += Instance_OnAnyUnitMovedGridPosition;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Instance_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void Instance_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    public void HideAllGridPositions()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingles[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x < range; x++)
        {
            for (int z = -range; z < range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }
        ShowGridPositionlist(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionlist(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingles[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPositions();

        Unit unit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        if (selectedAction != null)
        {
            GridVisualType gridVisualType;
            switch (selectedAction)
            {
                case MoveAction moveAction:
                    gridVisualType = GridVisualType.White;
                    break;
                case SpinAction spinAction:
                    gridVisualType = GridVisualType.Yellow;
                    break;
                case ShootAction shootAction:
                    gridVisualType = GridVisualType.Red;
                    ShowGridPositionRange(unit.GetGridPosition(), shootAction.GetMaximumShootDistance(), GridVisualType.RedSoft);
                    break;
                default:
                    gridVisualType = GridVisualType.White;
                    break;
            }

            ShowGridPositionlist(selectedAction.GetValidActionGridPositionList(), gridVisualType);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterials)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType: " + gridVisualType);
        return null;
    }
}
