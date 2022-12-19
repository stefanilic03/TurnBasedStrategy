using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnEndMoving;

    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 4f;
    [SerializeField] private int maxMoveDistance = 4;

    private Vector3 targetPosition;
    private float stoppingDistance = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update()
    {
        if (!isActive) { return; }

        //Character rotation
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, turnSpeed * Time.deltaTime);

        //Character actual movement
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            CharacterMovement(moveDirection);
            return;
        }

        OnEndMoving?.Invoke(this, EventArgs.Empty);

        ActionComplete();
    }

    //public void Move(GridPosition gridPosition, Action onActionComplete)
    //{
    //    this.onActionComplete = onActionComplete;
    //    isActive = true;
    //    this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
    //}

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
        //this.onActionComplete = onActionComplete;
        //isActive = true;
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

        OnStartMoving?.Invoke(this, EventArgs.Empty);
    }

    private void CharacterMovement(Vector3 moveDirection)
    {  
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        //animator.SetBool("isWalking", true);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                //if (unitGridPosition == testGridPosition)
                //{
                //    continue;
                //}
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Move";
    }
}
