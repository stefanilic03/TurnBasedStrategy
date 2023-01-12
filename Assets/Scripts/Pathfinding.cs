using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;

    [SerializeField] private Transform pathfindingGridDebugObjectPrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private int width;
    private int height;
    private float cellSize;

    private GridSystem<PathNode> gridSystem;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        //gridSystem = new GridSystem<PathNode>(45, 29, 2f, (GridSystem<PathNode> gameObject, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObjects(pathfindingGridDebugObjectPrefab);
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromPathNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        //No path found
        pathLength = 0;
        return null;
    }

    public void Setup(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridSystem = new GridSystem<PathNode>(width, height, cellSize, (GridSystem<PathNode> gridSystem, GridPosition gridPosition) => new PathNode(gridPosition));
        //gridSystem.CreateDebugObjects(pathfindingGridDebugObjectPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;
                if (Physics.Raycast(worldPosition + Vector3.down * raycastOffsetDistance, Vector3.up, raycastOffsetDistance * 2, obstaclesLayerMask))
                {
                    GetNodeAtPosition(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;

        while (currentNode.GetCameFromPathNode() != null)
        {
            pathNodeList.Add(currentNode.GetCameFromPathNode());
            currentNode = currentNode.GetCameFromPathNode();
        }

        pathNodeList.Reverse();

        List<GridPosition> gridPositionlist = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList)
        {
            gridPositionlist.Add(pathNode.GetGridPosition());
        }

        return gridPositionlist;
    }

    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance + MOVE_STRAIGHT_COST * remaining);
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost())
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private List<PathNode> GetNeighbourList(PathNode pathNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = pathNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0)
        {
            //Left neighbour
            neighbourList.Add(GetNodeAtPosition(gridPosition.x - 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0)
            {
                //Left down
                neighbourList.Add(GetNodeAtPosition(gridPosition.x - 1, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 <= gridSystem.GetHeight())
            {
                //Left up
                neighbourList.Add(GetNodeAtPosition(gridPosition.x - 1, gridPosition.z + 1));
            }
        }
        if (gridPosition.x + 1 <= gridSystem.GetWidth())
        {
            //Right neighbour
            neighbourList.Add(GetNodeAtPosition(gridPosition.x + 1, gridPosition.z + 0));
            if (gridPosition.z - 1 >= 0)
            {
                //Right down
                neighbourList.Add(GetNodeAtPosition(gridPosition.x + 1, gridPosition.z - 1));
            }
            if (gridPosition.z + 1 <= gridSystem.GetHeight())
            {
                //Right up
                neighbourList.Add(GetNodeAtPosition(gridPosition.x + 1, gridPosition.z + 1));
            }
        }
        if (gridPosition.z - 1 >= 0)
        {
            //Down neighbour
            neighbourList.Add(GetNodeAtPosition(gridPosition.x + 0, gridPosition.z - 1));
        }
        if (gridPosition.z + 1 <= gridSystem.GetHeight())
        {
            //Up neighbour
            neighbourList.Add(GetNodeAtPosition(gridPosition.x + 0, gridPosition.z + 1));
        }
        return neighbourList;
    }

    private PathNode GetNodeAtPosition(int x, int z)
    {
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return gridSystem.GetGridObject(gridPosition).IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;

    }
}
